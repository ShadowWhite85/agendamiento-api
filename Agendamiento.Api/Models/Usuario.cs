namespace Agendamiento.Api.Models;

public enum RolUsuario
{
    Admin = 0,
    Recepcionista = 1,
}

public class Usuario
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public RolUsuario Rol { get; set; } = RolUsuario.Recepcionista;
}
