using Agendamiento.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Agendamiento.Api.Data;

public class AgendamientoDbContext(DbContextOptions<AgendamientoDbContext> options) : DbContext(options)
{
    public DbSet<Cita> Citas => Set<Cita>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cita>(c =>
        {
            c.Property(x => x.ClienteNombre).HasMaxLength(120);
            c.Property(x => x.ClienteTelefono).HasMaxLength(20);
            c.Property(x => x.Servicio).HasMaxLength(120);
            c.HasIndex(x => x.FechaHora);
        });

        modelBuilder.Entity<Usuario>(u =>
        {
            u.HasIndex(x => x.Email).IsUnique();
            u.Property(x => x.Email).HasMaxLength(160);
            u.Property(x => x.Nombre).HasMaxLength(120);
        });

        // Los hashes son fijos (no BCrypt.HashPassword en runtime) y este seed es estático:
        // cualquier valor dinámico en HasData dispara PendingModelChangesWarning
        // (patrón validado en facturacion-app). Las citas se siembran al arrancar.
        modelBuilder.Entity<Usuario>().HasData(
            new Usuario { Id = 1, Email = "admin@agendamiento.app", Nombre = "Administrador Demo", Rol = RolUsuario.Admin, PasswordHash = "$2a$11$kSg0JBib7cgq.yS9QbDwf.iy5dEAA0WjyWYLjmEh.S9qm//nqIg4S" },
            new Usuario { Id = 2, Email = "recepcion@agendamiento.app", Nombre = "Recepción Demo", Rol = RolUsuario.Recepcionista, PasswordHash = "$2a$11$bTxBxKMu8hdpf59zEjQMpuXy2WdFWVCPDDLRXhSo5bWSAIK8iRQ7q" });
    }

    // Citas de demostración con fechas relativas (se llama tras MigrateAsync;
    // fuera de HasData para evitar modelos dinámicos)
    public static async Task SembrarCitasAsync(AgendamientoDbContext db)
    {
        if (await db.Citas.AnyAsync()) return;

        var hoy = DateTime.Today;
        db.Citas.AddRange(
            new Cita { ClienteNombre = "María López", ClienteTelefono = "0987654321", Servicio = "Corte y peinado", FechaHora = hoy.AddDays(1).AddHours(9), Estado = EstadoCita.Confirmada },
            new Cita { ClienteNombre = "Juan Pérez", ClienteTelefono = "0991234567", Servicio = "Consulta médica general", FechaHora = hoy.AddDays(1).AddHours(11), Estado = EstadoCita.Pendiente },
            new Cita { ClienteNombre = "Ana Torres", ClienteTelefono = "0965551234", Servicio = "Limpieza dental", FechaHora = hoy.AddDays(2).AddHours(15), Estado = EstadoCita.Pendiente },
            new Cita { ClienteNombre = "Carlos Ruiz", ClienteTelefono = "0954448877", Servicio = "Masaje terapéutico", FechaHora = hoy.AddDays(-1).AddHours(10), Estado = EstadoCita.Completada });
        await db.SaveChangesAsync();
    }
}
