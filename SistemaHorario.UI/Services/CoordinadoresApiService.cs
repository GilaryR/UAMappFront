using SistemaHorario.UI.Models.UI;
using SistemaHorarios.Application.Common;

namespace SistemaHorario.UI.Services;

public class UsuarioBackendDto
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

public class CoordinadoresApiService
{
    private readonly ApiClient _api = new();

    public async Task<ApiResponse<List<CoordinadorItem>>> ObtenerCoordinadoresAsync()
    {
        ApiResponse<List<UsuarioBackendDto>> resp =
            await _api.GetAsync<List<UsuarioBackendDto>>("usuarios");

        if (!resp.Success || resp.Data == null)
        {
            return new ApiResponse<List<CoordinadorItem>>
            {
                Success = false,
                Message = string.IsNullOrWhiteSpace(resp.Message)
                    ? "No se pudieron consultar los usuarios."
                    : resp.Message,
                Data = new List<CoordinadorItem>()
            };
        }

        List<CoordinadorItem> lista = resp.Data
            .Where(u => u.Rol.Contains(
                "Coordinador",
                StringComparison.OrdinalIgnoreCase))
            .Select(MapearCoordinador)
            .ToList();

        return new ApiResponse<List<CoordinadorItem>>
        {
            Success = true,
            Message = "Coordinadores obtenidos correctamente.",
            Data = lista
        };
    }

    public async Task<ApiResponse<string>> CrearCoordinadorAsync(
        CoordinadorItem coordinador)
    {
        return await _api.PostAsync("usuarios", new
        {
            NombreCompleto = coordinador.NombreCompleto,
            Cedula = coordinador.Cedula,
            CorreoInstitucional = coordinador.CorreoInstitucional,
            Contrasena = "Coordinador123!",
            IdRol = 2,
            Estado = coordinador.Estado,
            Celular = coordinador.Celular
        });
    }

    public async Task<ApiResponse<string>> ActualizarCoordinadorAsync(
        CoordinadorItem coordinador)
    {
        return await _api.PutAsync($"usuarios/{coordinador.IdCoordinador}", new
        {
            NombreCompleto = coordinador.NombreCompleto,
            Cedula = coordinador.Cedula,
            CorreoInstitucional = coordinador.CorreoInstitucional,
            IdRol = 2,
            Estado = coordinador.Estado,
            Celular = coordinador.Celular
        });
    }

    public async Task<ApiResponse<string>> EliminarCoordinadorAsync(int id)
    {
        return await _api.DeleteAsync<string>($"usuarios/{id}");
    }

    private static CoordinadorItem MapearCoordinador(UsuarioBackendDto usuario)
    {
        return new CoordinadorItem
        {
            IdCoordinador = usuario.IdUsuario,
            NombreCompleto = usuario.NombreCompleto,
            Cedula = usuario.Cedula,
            CorreoInstitucional = usuario.CorreoInstitucional,
            Rol = usuario.Rol,
            Estado = usuario.Estado,
            Celular = usuario.Celular
        };
    }
}
