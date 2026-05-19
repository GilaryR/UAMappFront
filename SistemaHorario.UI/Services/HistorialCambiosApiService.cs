using SistemaHorario.UI.Models.UI;
using SistemaHorarios.Application.Common;

namespace SistemaHorario.UI.Services;

public class HistorialCambioBackendDto
{
    public int IdCambio { get; set; }
    public string Fecha { get; set; } = string.Empty;
    public string Hora { get; set; } = string.Empty;
    public string Usuario { get; set; } = string.Empty;
    public string Modulo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
}

public class HistorialCambiosApiService
{
    private readonly ApiClient _api = new();

    public async Task<ApiResponse<List<HistorialCambioItem>>> ObtenerHistorialAsync(
        string? usuario = null,
        string? modulo = null,
        DateTime? fechaDesde = null,
        DateTime? fechaHasta = null)
    {
        string ruta = "historial-cambios";
        var query = new List<string>();

        if (!string.IsNullOrWhiteSpace(usuario) && usuario != "Todos")
            query.Add($"usuario={Uri.EscapeDataString(usuario)}");

        if (!string.IsNullOrWhiteSpace(modulo) && modulo != "Todos")
            query.Add($"modulo={Uri.EscapeDataString(modulo)}");

        if (fechaDesde.HasValue)
            query.Add($"fechaDesde={fechaDesde.Value:yyyy-MM-dd}");

        if (fechaHasta.HasValue)
            query.Add($"fechaHasta={fechaHasta.Value:yyyy-MM-dd}");

        if (query.Count > 0)
            ruta += "?" + string.Join("&", query);

        var resp = await _api.GetAsync<List<HistorialCambioBackendDto>>(ruta);
        if (!resp.Success || resp.Data == null)
            return new ApiResponse<List<HistorialCambioItem>> { Success = false, Message = resp.Message };

        var lista = resp.Data
            .Select(MapearHistorial)
            .OrderByDescending(x => x.FechaHora)
            .ToList();

        return new ApiResponse<List<HistorialCambioItem>> { Success = true, Data = lista };
    }

    public async Task<ApiResponse<List<string>>> ObtenerUsuariosAsync()
    {
        var resp = await _api.GetAsync<List<string>>("historial-cambios/usuarios");
        if (!resp.Success || resp.Data == null)
            return new ApiResponse<List<string>> { Success = false, Message = resp.Message };

        return new ApiResponse<List<string>> { Success = true, Data = resp.Data };        
    }

    public async Task<ApiResponse<List<string>>> ObtenerModulosAsync()
    {
        var resp = await _api.GetAsync<List<string>>("historial-cambios/modulos");
        if (!resp.Success || resp.Data == null)
            return new ApiResponse<List<string>> { Success = false, Message = resp.Message };

        return new ApiResponse<List<string>> { Success = true, Data = resp.Data };        
    }

    private HistorialCambioItem MapearHistorial(HistorialCambioBackendDto dto)
    {
        DateTime fechaHora = DateTime.TryParseExact(
            $"{dto.Fecha} {dto.Hora}",
            "yyyy-MM-dd HH:mm",
            null,
            System.Globalization.DateTimeStyles.None,
            out DateTime resultado)
            ? resultado
            : DateTime.UtcNow;

        return new HistorialCambioItem
        {
            IdHistorial = dto.IdCambio,
            FechaHora = fechaHora,
            Usuario = dto.Usuario,
            Modulo = dto.Modulo,
            Descripcion = dto.Descripcion
        };
    }
}
