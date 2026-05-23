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
    public string? FotoPerfilUrl { get; set; }
}

public class PerfilApiService
{
    private const string UrlBackend = "http://localhost:5023";

    private readonly ApiClient _api = new();

    public async Task<ApiResponse<PerfilUsuarioItem>> ObtenerPerfilAsync()
    {
        var resp = await _api.GetAsync<PerfilBackendDto>("usuarios/perfil");

        if (!resp.Success || resp.Data == null)
        {
            return new ApiResponse<PerfilUsuarioItem>
            {
                Success = false,
                Message = resp.Message
            };
        }

        var perfil = new PerfilUsuarioItem
        {
            NombreCompleto = resp.Data.NombreCompleto,
            CorreoInstitucional = resp.Data.CorreoInstitucional,
            Rol = resp.Data.Rol,
            Telefono = resp.Data.Celular,
            FacultadPrograma = "Universidad Autónoma de Manizales",
            RutaImagen = ResolverRutaFoto(resp.Data.FotoPerfilUrl)
        };

        return new ApiResponse<PerfilUsuarioItem>
        {
            Success = true,
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
        string actual,
        string nueva)
    {
        return await _api.PutAsync("usuarios/cambiar-contrasena", new
        {
            ContrasenaActual = actual,
            NuevaContrasena = nueva
        });
    }

    public async Task<ApiResponse<string>> ActualizarFotoPerfilAsync(
        string rutaArchivo)
    {
        return await _api.PutFileAsync<string>(
            "usuarios/perfil/foto",
            "foto",
            rutaArchivo
        );
    }

    private static string ResolverRutaFoto(string? fotoPerfilUrl)
    {
        if (string.IsNullOrWhiteSpace(fotoPerfilUrl))
        {
            return "/Assets/Images/ImgUsuario.png";
        }

        if (fotoPerfilUrl.StartsWith("http"))
        {
            return fotoPerfilUrl;
        }

        return UrlBackend + fotoPerfilUrl;
    }
}