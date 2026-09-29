using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Agendamiento.Tests;

public class ApiFactory : WebApplicationFactory<Program>
{
    private readonly string _dbPath =
        Path.Combine(Path.GetTempPath(), $"agendamiento-test-{Guid.NewGuid():N}.db");

    protected override void ConfigureWebHost(IWebHostBuilder builder) =>
        builder.UseSetting("ConnectionStrings:DefaultConnection", $"Data Source={_dbPath}");

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        foreach (var suf in new[] { "", "-wal", "-shm" })
        {
            try { File.Delete(_dbPath + suf); } catch { /* mejor esfuerzo */ }
        }
    }
}

public class ApiTests(ApiFactory factory) : IClassFixture<ApiFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    private async Task<string> LoginAsync(string email, string password)
    {
        var resp = await _client.PostAsJsonAsync("/api/auth/login", new { email, password });
        Assert.Equal(HttpStatusCode.OK, resp.StatusCode);
        var body = await resp.Content.ReadFromJsonAsync<Agendamiento.Api.Dtos.TokenResponse>();
        Assert.False(string.IsNullOrEmpty(body!.Token));
        return body.Token;
    }

    private void Usar(string token) =>
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

    [Fact]
    public async Task Login_CredencialesValidas_RetornaToken()
    {
        var token = await LoginAsync("admin@agendamiento.app", "admin123");
        Assert.Contains('.', token);
    }

    [Fact]
    public async Task Citas_SinToken_Return401()
    {
        var resp = await _client.GetAsync("/api/citas");
        Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
    }

    [Fact]
    public async Task Login_CredencialesInvalidas_Return401()
    {
        var resp = await _client.PostAsJsonAsync("/api/auth/login",
            new { email = "admin@agendamiento.app", password = "incorrecta" });
        Assert.Equal(HttpStatusCode.Unauthorized, resp.StatusCode);
    }

    [Fact]
    public async Task CrearCita_ConToken_LuegoApareceEnElListado()
    {
        Usar(await LoginAsync("admin@agendamiento.app", "admin123"));

        var crear = await _client.PostAsJsonAsync("/api/citas", new
        {
            clienteNombre = "Cliente de Prueba",
            clienteTelefono = "0911112222",
            servicio = "Corte de prueba",
            fechaHora = DateTime.Today.AddDays(3).AddHours(10),
        });
        Assert.Equal(HttpStatusCode.Created, crear.StatusCode);
        var cita = await crear.Content.ReadFromJsonAsync<Agendamiento.Api.Dtos.CitaResponse>();
        Assert.Equal("Cliente de Prueba", cita!.ClienteNombre);

        var lista = await _client.GetFromJsonAsync<List<Agendamiento.Api.Dtos.CitaResponse>>("/api/citas");
        Assert.NotNull(lista);
        Assert.Contains(lista, c => c.ClienteNombre == "Cliente de Prueba");
    }

    [Fact]
    public async Task EliminarCita_ComoRecepcionista_Return403()
    {
        Usar(await LoginAsync("recepcion@agendamiento.app", "recepcion123"));

        var resp = await _client.DeleteAsync("/api/citas/1");
        Assert.Equal(HttpStatusCode.Forbidden, resp.StatusCode);
    }

    [Fact]
    public async Task EliminarCita_ComoAdmin_Return204()
    {
        Usar(await LoginAsync("admin@agendamiento.app", "admin123"));

        var crear = await _client.PostAsJsonAsync("/api/citas", new
        {
            clienteNombre = "Se Elimina",
            clienteTelefono = "0900000001",
            servicio = "Temporal",
            fechaHora = DateTime.Today.AddDays(5),
        });
        var cita = await crear.Content.ReadFromJsonAsync<Agendamiento.Api.Dtos.CitaResponse>();

        var resp = await _client.DeleteAsync($"/api/citas/{cita!.Id}");
        Assert.Equal(HttpStatusCode.NoContent, resp.StatusCode);
    }
}
