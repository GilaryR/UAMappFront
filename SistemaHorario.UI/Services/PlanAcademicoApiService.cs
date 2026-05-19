using SistemaHorario.UI.Models.UI;
using SistemaHorarios.Application.Common;

namespace SistemaHorario.UI.Services;

public class PlanAcademicoBackendDto
{
    public int IdPlanAcademico { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Programa { get; set; } = string.Empty;
    public int Anio { get; set; }
    public string Estado { get; set; } = string.Empty;
    public List<SemestreBackendDto>? Semestres { get; set; }
}

public class SemestreBackendDto
{
    public int IdSemestrePlan { get; set; }
    public int NumeroSemestre { get; set; }
    public List<object>? Materias { get; set; }
}

public class PlanAcademicoApiService
{
    private readonly ApiClient _api = new();

    public async Task<ApiResponse<List<PlanAcademicoItem>>> ObtenerPlanesAsync()
    {
        var resp = await _api.GetAsync<List<PlanAcademicoBackendDto>>("PlanAcademico");
        if (!resp.Success || resp.Data == null)
            return new ApiResponse<List<PlanAcademicoItem>> { Success = false, Message = resp.Message };

        var lista = resp.Data.Select(p => new PlanAcademicoItem
        {
            IdPlanAcademico = p.IdPlanAcademico,
            Nombre = p.Nombre,
            Jornada = p.Programa,
            CargaPorSemestre = "Por definir",
            TotalSemestres = p.Semestres?.Count ?? 0,
            TotalMaterias = p.Semestres?.Sum(s => s.Materias?.Count ?? 0) ?? 0,
            TotalCreditos = 0
        }).ToList();

        return new ApiResponse<List<PlanAcademicoItem>> { Success = true, Data = lista };
    }

    public async Task<ApiResponse<PlanAcademicoBackendDto>> CrearPlanAsync(PlanAcademicoItem p)
        => await _api.PostAsync<PlanAcademicoBackendDto>("PlanAcademico", new
        {
            Nombre = p.Nombre,
            Programa = string.IsNullOrWhiteSpace(p.Jornada) ? "General" : p.Jornada,
            Anio = DateTime.Now.Year,
            Estado = "Activo"
        });

    public async Task<ApiResponse<string>> ActualizarPlanAsync(PlanAcademicoItem p)
        => await _api.PutAsync($"PlanAcademico/{p.IdPlanAcademico}", new
        {
            Nombre = p.Nombre,
            Programa = string.IsNullOrWhiteSpace(p.Jornada) ? "General" : p.Jornada,
            Anio = DateTime.Now.Year,
            Estado = "Activo"
        });

    public async Task<ApiResponse<string>> EliminarPlanAsync(int id)
        => await _api.DeleteAsync($"PlanAcademico/{id}");
}
