using SistemaHorario.UI.Models.UI;
using SistemaHorarios.Application.Common;

namespace SistemaHorario.UI.Services;

public class PerfilBackendDto
{
    public int IdUsuario { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public string Cedula { get; set; } = string.Empty;
    public string CorreoInstitucional { get; set; } = string.Empty;
    public int IdRol { get; set; }
    public string Rol { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public string Celular { get; set; } = string.Empty;
}

public class PerfilApiService
{
    private readonly ApiClient _api = new();

    public async Task<ApiResponse<PerfilUsuarioItem>> ObtenerPerfilAsync()
    {
        ApiResponse<PerfilBackendDto> resp =
            await _api.GetAsync<PerfilBackendDto>("usuarios/perfil");

        if (!resp.Success || resp.Data == null)
        {
            return new ApiResponse<PerfilUsuarioItem>
            {
                Success = false,
                Message = string.IsNullOrWhiteSpace(resp.Message)
                    ? "No se pudo cargar el perfil del usuario."
                    : resp.Message
            };
        }

        PerfilUsuarioItem perfil = new PerfilUsuarioItem
        {
            NombreCompleto = resp.Data.NombreCompleto,
            CorreoInstitucional = resp.Data.CorreoInstitucional,
            Rol = resp.Data.Rol,
            Telefono = resp.Data.Celular,
            FacultadPrograma = "Universidad Autónoma de Manizales",
            RutaImagen = "/Assets/Images/ImgUsuario.png"
        };

        return new ApiResponse<PerfilUsuarioItem>
        {
            Success = true,
            Message = "Perfil cargado correctamente.",
            Data = perfil
        };
    }

    public async Task<ApiResponse<string>> ActualizarPerfilAsync(
        string nombre,
        string correo,
        string celular)
    {
        return await _api.PutAsync("usuarios/perfil", new
        {
            NombreCompleto = nombre,
            CorreoInstitucional = correo,
            Celular = celular
        });
    }

    public async Task<ApiResponse<string>> CambiarContrasenaAsync(
        string contrasenaActual,
        string nuevaContrasena)
    {
        return await _api.PutAsync("usuarios/cambiar-contrasena", new
        {
            ContrasenaActual = contrasenaActual,
            NuevaContrasena = nuevaContrasena
        });
    }
}