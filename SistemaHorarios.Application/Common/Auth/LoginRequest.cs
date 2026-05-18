namespace SistemaHorarios.Application.Common.Auth;

public class LoginRequest
{
    public string CorreoInstitucional { get; set; } = string.Empty;

    public string Contrasena { get; set; } = string.Empty;
}