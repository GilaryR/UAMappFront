using SistemaHorarios.Application.Common;

namespace SistemaHorario.UI.Services;

public class DashboardBackendDto
{
    public int TotalUsuarios { get; set; }
    public int TotalRoles { get; set; }
    public int TotalMaterias { get; set; }
    public int TotalPrerrequisitos { get; set; }
    public int TotalFranjasHorarias { get; set; }
    public int TotalGrupos { get; set; }
    public int TotalPlanesAcademicos { get; set; }
    public int TotalSemestresPlan { get; set; }
    public int TotalMateriasPlan { get; set; }
}

public class DashboardApiService
{
    private readonly ApiClient _api = new();

    public async Task<ApiResponse<DashboardBackendDto>> ObtenerResumenAsync()
        => await _api.GetAsync<DashboardBackendDto>("dashboard/resumen");
}
