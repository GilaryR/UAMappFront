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

public class RolBackendDto
{
    public int IdRol { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public bool Activo { get; set; }
}

public class CoordinadoresApiService
{
    private readonly ApiClient _api = new();

    public async Task<ApiResponse<List<CoordinadorItem>>> ObtenerCoordinadoresAsync()
    {
        ApiResponse<List<UsuarioBackendDto>> resp =
            await _api.GetAsync<List<UsuarioBackendDto>>("usuarios/coordinadores");

        if (!resp.Success || resp.Data == null)
        {
            return new ApiResponse<List<CoordinadorItem>>
            {
                Success = false,
                Message = string.IsNullOrWhiteSpace(resp.Message)
                    ? "No se pudieron consultar los coordinadores."
                    : resp.Message,
                Data = new List<CoordinadorItem>()
            };
        }

        List<CoordinadorItem> lista = resp.Data
            .Select(MapearCoordinador)
            .OrderBy(x => x.Estado == "Inactivo")
            .ThenBy(x => x.NombreCompleto)
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
        int idRolCoordinador = await ObtenerIdRolCoordinadorAsync();

        string contrasena = string.IsNullOrWhiteSpace(coordinador.ContrasenaInicial)
            ? "Coordinador123!"
            : coordinador.ContrasenaInicial.Trim();

        return await _api.PostAsync("usuarios", new
        {
            NombreCompleto = coordinador.NombreCompleto,
            Cedula = coordinador.Cedula,
            CorreoInstitucional = coordinador.CorreoInstitucional,
            Contrasena = contrasena,
            IdRol = idRolCoordinador,
            Estado = coordinador.Estado,
            Celular = coordinador.Celular
        });
    }

    public async Task<ApiResponse<string>> ActualizarCoordinadorAsync(
        CoordinadorItem coordinador)
    {
        int idRolCoordinador = await ObtenerIdRolCoordinadorAsync();

        return await _api.PutAsync($"usuarios/{coordinador.IdCoordinador}", new
        {
            NombreCompleto = coordinador.NombreCompleto,
            Cedula = coordinador.Cedula,
            CorreoInstitucional = coordinador.CorreoInstitucional,
            IdRol = idRolCoordinador,
            Estado = coordinador.Estado,
            Celular = coordinador.Celular
        });
    }

    public async Task<ApiResponse<string>> CambiarEstadoCoordinadorAsync(
        int id,
        string estado)
    {
        return await _api.PutAsync($"usuarios/{id}/estado", new
        {
            Estado = estado
        });
    }

    public async Task<ApiResponse<string>> InactivarCoordinadorAsync(int id)
    {
        return await CambiarEstadoCoordinadorAsync(id, "Inactivo");
    }

    public async Task<ApiResponse<string>> ActivarCoordinadorAsync(int id)
    {
        return await CambiarEstadoCoordinadorAsync(id, "Activo");
    }

    public async Task<ApiResponse<string>> EliminarCoordinadorAsync(int id)
    {
        // Se conserva por compatibilidad: eliminar equivale a inactivar.
        return await InactivarCoordinadorAsync(id);
    }

    private async Task<int> ObtenerIdRolCoordinadorAsync()
    {
        ApiResponse<List<RolBackendDto>> resp =
            await _api.GetAsync<List<RolBackendDto>>("roles");

        RolBackendDto? rolCoordinador = resp.Data?
            .FirstOrDefault(rol =>
                string.Equals(
                    rol.Nombre,
                    "Coordinador",
                    StringComparison.OrdinalIgnoreCase));

        return rolCoordinador?.IdRol ?? 2;
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
            Estado = NormalizarEstado(usuario.Estado),
            Celular = usuario.Celular
        };
    }

    private static string NormalizarEstado(string estado)
    {
        return string.Equals(
            estado?.Trim(),
            "Inactivo",
            StringComparison.OrdinalIgnoreCase)
                ? "Inactivo"
                : "Activo";
    }
}
