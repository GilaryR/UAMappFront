using SistemaHorarios.Application.Common;
using System.Collections.Generic;
using System.Linq;

namespace SistemaHorario.UI.Services;

public class ReporteTipoBackendDto
{
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
}

public class ReporteGeneralBackendDto
{
    public int TotalUsuarios { get; set; }
    public int TotalRoles { get; set; }
    public int TotalMaterias { get; set; }
    public int TotalPrerrequisitos { get; set; }
    public int TotalFranjasHorarias { get; set; }
    public int TotalPlanesAcademicos { get; set; }
    public int TotalSemestresPlan { get; set; }
    public int TotalMateriasPlan { get; set; }
}

public class ReporteUsuariosPorRolBackendDto
{
    public string Rol { get; set; } = string.Empty;
    public int TotalUsuarios { get; set; }
}

public class ReporteMateriasPorSemestreBackendDto
{
    public int Semestre { get; set; }
    public int TotalMaterias { get; set; }
    public List<string> Materias { get; set; } = new();
}

public class ReporteFranjaHorariaBackendDto
{
    public string Dia { get; set; } = string.Empty;
    public int TotalFranjas { get; set; }
}

public class ReportePlanAcademicoBackendDto
{
    public int IdPlanAcademico { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Programa { get; set; } = string.Empty;
    public int Anio { get; set; }
    public int TotalSemestres { get; set; }
    public int TotalMaterias { get; set; }
}

public class ReportesApiService
{
    private readonly ApiClient _api = new();

    public async Task<ApiResponse<List<string>>> ObtenerTiposReporteAsync()
    {
        var resp = await _api.GetAsync<List<ReporteTipoBackendDto>>("catalogos/tipos-reporte");
        if (!resp.Success || resp.Data == null)
            return new ApiResponse<List<string>> { Success = false, Message = resp.Message };

        var tipos = resp.Data.Select(x => x.Nombre).ToList();
        return new ApiResponse<List<string>> { Success = true, Data = tipos };
    }

    public async Task<ApiResponse<List<string>>> ObtenerFormatosReporteAsync()
        => await _api.GetAsync<List<string>>("catalogos/formatos-reporte");

    public async Task<ApiResponse<List<int>>> ObtenerSemestresAsync(string? jornada = null)
    {
        string ruta = "catalogos/semestres";
        if (!string.IsNullOrWhiteSpace(jornada))
            ruta += $"?jornada={Uri.EscapeDataString(jornada)}";

        return await _api.GetAsync<List<int>>(ruta);
    }

    public async Task<ApiResponse<ReporteGeneralBackendDto>> ObtenerReporteGeneralAsync()
        => await _api.GetAsync<ReporteGeneralBackendDto>("reportes/general");

    public async Task<ApiResponse<List<ReporteUsuariosPorRolBackendDto>>> ObtenerUsuariosPorRolAsync()
        => await _api.GetAsync<List<ReporteUsuariosPorRolBackendDto>>("reportes/usuarios-por-rol");

    public async Task<ApiResponse<List<ReporteMateriasPorSemestreBackendDto>>> ObtenerMateriasPorSemestreAsync()
        => await _api.GetAsync<List<ReporteMateriasPorSemestreBackendDto>>("reportes/materias-por-semestre");

    public async Task<ApiResponse<List<ReporteFranjaHorariaBackendDto>>> ObtenerFranjasPorDiaAsync()
        => await _api.GetAsync<List<ReporteFranjaHorariaBackendDto>>("reportes/franjas-por-dia");

    public async Task<ApiResponse<List<ReportePlanAcademicoBackendDto>>> ObtenerPlanesAcademicosAsync()
        => await _api.GetAsync<List<ReportePlanAcademicoBackendDto>>("reportes/planes-academicos");
}
