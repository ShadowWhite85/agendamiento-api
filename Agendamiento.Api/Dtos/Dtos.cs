using Agendamiento.Api.Models;

namespace Agendamiento.Api.Dtos;

public record LoginRequest(string Email, string Password);

public record TokenResponse(string Token, string Nombre, string Email, string Rol, DateTime Expira);

public record CrearCitaRequest(string ClienteNombre, string ClienteTelefono, string Servicio, DateTime FechaHora, string? Notas);

public record ActualizarCitaRequest(string ClienteNombre, string ClienteTelefono, string Servicio, DateTime FechaHora, EstadoCita Estado, string? Notas);

public record CitaResponse(int Id, string ClienteNombre, string ClienteTelefono, string Servicio, DateTime FechaHora, EstadoCita Estado, string? Notas, DateTime CreadaEn)
{
    public static CitaResponse From(Cita c) =>
        new(c.Id, c.ClienteNombre, c.ClienteTelefono, c.Servicio, c.FechaHora, c.Estado, c.Notas, c.CreadaEn);
}
