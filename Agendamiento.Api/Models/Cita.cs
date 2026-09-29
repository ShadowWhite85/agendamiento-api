namespace Agendamiento.Api.Models;

public enum EstadoCita
{
    Pendiente = 0,
    Confirmada = 1,
    Completada = 2,
    Cancelada = 3,
}

public class Cita
{
    public int Id { get; set; }
    public string ClienteNombre { get; set; } = string.Empty;
    public string ClienteTelefono { get; set; } = string.Empty;
    public string Servicio { get; set; } = string.Empty;
    public DateTime FechaHora { get; set; }
    public EstadoCita Estado { get; set; } = EstadoCita.Pendiente;
    public string? Notas { get; set; }
    public DateTime CreadaEn { get; set; } = DateTime.UtcNow;
}
