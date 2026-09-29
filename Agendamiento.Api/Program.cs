using System.Text;
using Agendamiento.Api.Data;
using Agendamiento.Api.Dtos;
using Agendamiento.Api.Models;
using Agendamiento.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AgendamientoDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<TokenService>();

// Swagger con soporte para "Authorize" (Bearer token) — público: es la demo del portafolio
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.ParameterLocation.Header,
        Description = "Pega tu token JWT (sin la palabra 'Bearer')"
    });
    options.AddSecurityRequirement(document => new Microsoft.OpenApi.OpenApiSecurityRequirement
    {
        [new Microsoft.OpenApi.OpenApiSecuritySchemeReference("Bearer", document)] = []
    });
});

// Autenticación JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

builder.Services.AddAuthorization(options =>
    options.AddPolicy("SoloAdmin", policy => policy.RequireRole("Admin")));

// CORS configurable: orígenes permitidos en appsettings
var orígenesPermitidos = builder.Configuration.GetSection("Cors:Origins").Get<string[]>() ?? [];
builder.Services.AddCors(options =>
    options.AddPolicy("Web", policy =>
        policy.WithOrigins(orígenesPermitidos)
              .AllowAnyHeader()
              .AllowAnyMethod()));

var app = builder.Build();

// Aplica las migraciones (incluye usuarios de demostración) al primer arranque
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AgendamientoDbContext>();
    await db.Database.MigrateAsync();
    await Agendamiento.Api.Data.AgendamientoDbContext.SembrarCitasAsync(db);
}

// Swagger público en todos los entornos (demo del portafolio)
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("Web");
app.UseAuthentication();
app.UseAuthorization();
// Sin UseHttpsRedirection: el TLS lo termina Render/Cloudflare en el borde

var api = app.MapGroup("/api").WithTags("API");

// ---- Auth ----
api.MapPost("/auth/login", async (LoginRequest dto, AgendamientoDbContext db, TokenService tokens) =>
{
    var usuario = await db.Usuarios.FirstOrDefaultAsync(u => u.Email == dto.Email);
    if (usuario is null || !BCrypt.Net.BCrypt.Verify(dto.Password, usuario.PasswordHash))
        return Results.Unauthorized();

    return Results.Ok(tokens.Crear(usuario));
}).AllowAnonymous();

// ---- Citas (CRUD) ----
var citas = api.MapGroup("/citas").RequireAuthorization();

citas.MapGet("/", async (AgendamientoDbContext db, EstadoCita? estado, DateTime? desde, DateTime? hasta) =>
{
    var query = db.Citas.AsNoTracking().AsQueryable();
    if (estado is not null) query = query.Where(c => c.Estado == estado);
    if (desde is not null) query = query.Where(c => c.FechaHora >= desde);
    if (hasta is not null) query = query.Where(c => c.FechaHora <= hasta);

    var lista = await query.OrderBy(c => c.FechaHora).ToListAsync();
    return Results.Ok(lista.Select(CitaResponse.From));
});

citas.MapGet("/{id:int}", async (int id, AgendamientoDbContext db) =>
{
    var cita = await db.Citas.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
    return cita is null ? Results.NotFound() : Results.Ok(CitaResponse.From(cita));
});

citas.MapPost("/", async (CrearCitaRequest dto, AgendamientoDbContext db) =>
{
    if (string.IsNullOrWhiteSpace(dto.ClienteNombre) || string.IsNullOrWhiteSpace(dto.Servicio))
        return Results.BadRequest(new { mensaje = "Cliente y servicio son obligatorios" });
    if (dto.FechaHora == default)
        return Results.BadRequest(new { mensaje = "Fecha y hora son obligatorias" });

    var cita = new Cita
    {
        ClienteNombre = dto.ClienteNombre.Trim(),
        ClienteTelefono = dto.ClienteTelefono.Trim(),
        Servicio = dto.Servicio.Trim(),
        FechaHora = dto.FechaHora,
        Notas = dto.Notas?.Trim(),
        Estado = EstadoCita.Pendiente,
    };
    db.Citas.Add(cita);
    await db.SaveChangesAsync();
    return Results.Created($"/api/citas/{cita.Id}", CitaResponse.From(cita));
});

citas.MapPut("/{id:int}", async (int id, ActualizarCitaRequest dto, AgendamientoDbContext db) =>
{
    if (string.IsNullOrWhiteSpace(dto.ClienteNombre) || string.IsNullOrWhiteSpace(dto.Servicio))
        return Results.BadRequest(new { mensaje = "Cliente y servicio son obligatorios" });
    if (!Enum.IsDefined(dto.Estado))
        return Results.BadRequest(new { mensaje = "Estado no válido" });

    var cita = await db.Citas.FindAsync(id);
    if (cita is null) return Results.NotFound();

    cita.ClienteNombre = dto.ClienteNombre.Trim();
    cita.ClienteTelefono = dto.ClienteTelefono.Trim();
    cita.Servicio = dto.Servicio.Trim();
    cita.FechaHora = dto.FechaHora;
    cita.Estado = dto.Estado;
    cita.Notas = dto.Notas?.Trim();
    await db.SaveChangesAsync();

    return Results.Ok(CitaResponse.From(cita));
});

citas.MapDelete("/{id:int}", async (int id, AgendamientoDbContext db) =>
{
    var cita = await db.Citas.FindAsync(id);
    if (cita is null) return Results.NotFound();

    db.Citas.Remove(cita);
    await db.SaveChangesAsync();
    return Results.NoContent();
}).RequireAuthorization("SoloAdmin");

app.Run();

public partial class Program { }
