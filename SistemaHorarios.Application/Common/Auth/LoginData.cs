namespace SistemaHorarios.Application.Common.Auth;

public class LoginData
{
    public int IdUsuario { get; set; }

    public string NombreCompleto { get; set; } = string.Empty;

    public string CorreoInstitucional { get; set; } = string.Empty;

    public string Rol { get; set; } = string.Empty;

    public string Token { get; set; } = string.Empty;
}