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
        var resp = await _api.GetAsync<List<UsuarioBackendDto>>("usuarios");
        if (!resp.Success || resp.Data == null)
            return new ApiResponse<List<CoordinadorItem>> { Success = false, Message = resp.Message };

        var lista = resp.Data
            .Where(u => u.Rol.Contains("Coordinador", StringComparison.OrdinalIgnoreCase))
            .Select(u => new CoordinadorItem
            {
                IdCoordinador = u.IdUsuario,
                NombreCompleto = u.NombreCompleto,
                Cedula = u.Cedula,
                CorreoInstitucional = u.CorreoInstitucional,
                Rol = u.Rol,
                Estado = u.Estado,
                Celular = u.Celular
            }).ToList();

        return new ApiResponse<List<CoordinadorItem>> { Success = true, Data = lista };
    }

    public async Task<ApiResponse<string>> CrearCoordinadorAsync(CoordinadorItem coordinador)
        => await _api.PostAsync("usuarios", new
        {
            NombreCompleto = coordinador.NombreCompleto,
            Cedula = coordinador.Cedula,
            CorreoInstitucional = coordinador.CorreoInstitucional,
            Contrasena = "Coordinador123!",
            IdRol = 2,
            Estado = coordinador.Estado,
            Celular = coordinador.Celular
        });

    public async Task<ApiResponse<string>> ActualizarCoordinadorAsync(CoordinadorItem coordinador)
        => await _api.PutAsync($"usuarios/{coordinador.IdCoordinador}", new
        {
            NombreCompleto = coordinador.NombreCompleto,
            Cedula = coordinador.Cedula,
            CorreoInstitucional = coordinador.CorreoInstitucional,
            IdRol = 2,
            Estado = coordinador.Estado,
            Celular = coordinador.Celular
        });

    public async Task<ApiResponse<string>> EliminarCoordinadorAsync(int id)
        => await _api.DeleteAsync($"usuarios/{id}");
}
