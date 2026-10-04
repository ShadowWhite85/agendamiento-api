namespace Agendamiento.Api.Services;

/// <summary>
/// Reglas de entrada de una cita. SQLite no aplica los MaxLength del modelo,
/// así que los límites se validan aquí antes de guardar.
/// </summary>
public static class ValidacionCita
{
    public const int MaxNombre = 120;
    public const int MaxTelefono = 20;
    public const int MaxServicio = 120;
    public const int MaxNotas = 500;

    /// <summary>Devuelve el mensaje de error, o null si la cita es válida.</summary>
    public static string? Validar(string? clienteNombre, string? clienteTelefono, string? servicio, string? notas)
    {
        if (string.IsNullOrWhiteSpace(clienteNombre) || string.IsNullOrWhiteSpace(servicio))
            return "Cliente y servicio son obligatorios";
        if (string.IsNullOrWhiteSpace(clienteTelefono))
            return "El teléfono del cliente es obligatorio";
        if (clienteNombre.Trim().Length > MaxNombre)
            return $"El nombre del cliente admite máximo {MaxNombre} caracteres";
        if (clienteTelefono.Trim().Length > MaxTelefono)
            return $"El teléfono admite máximo {MaxTelefono} caracteres";
        if (servicio.Trim().Length > MaxServicio)
            return $"El servicio admite máximo {MaxServicio} caracteres";
        if (notas is not null && notas.Trim().Length > MaxNotas)
            return $"Las notas admiten máximo {MaxNotas} caracteres";
        return null;
    }
}
