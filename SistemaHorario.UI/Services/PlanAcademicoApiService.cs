using SistemaHorario.UI.Models.UI;
using SistemaHorarios.Application.Common;
using System.Collections.ObjectModel;

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
    public List<MateriaPlanBackendDto>? Materias { get; set; }
}

public class MateriaPlanBackendDto
{
    public int IdMateriaPlan { get; set; }
    public int IdMateria { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public int Creditos { get; set; }
    public int IntensidadHorariaSemanal { get; set; }
}

public class PlanAcademicoApiService
{
    private readonly ApiClient _api = new();

    public async Task<ApiResponse<List<PlanAcademicoItem>>> ObtenerPlanesAsync()
    {
        var resp = await _api.GetAsync<List<PlanAcademicoBackendDto>>("PlanAcademico");
        if (!resp.Success || resp.Data == null)
            return new ApiResponse<List<PlanAcademicoItem>> { Success = false, Message = resp.Message };

        return new ApiResponse<List<PlanAcademicoItem>>
        {
            Success = true,
            Data = resp.Data.Select(MapearItem).ToList()
        };
    }

    public async Task<ApiResponse<PlanAcademicoItem>> ObtenerPlanPorIdAsync(int id)
    {
        var resp = await _api.GetAsync<PlanAcademicoBackendDto>($"PlanAcademico/{id}");
        if (!resp.Success || resp.Data == null)
            return new ApiResponse<PlanAcademicoItem> { Success = false, Message = resp.Message };

        return new ApiResponse<PlanAcademicoItem> { Success = true, Data = MapearItem(resp.Data) };
    }

    public async Task<ApiResponse<PlanAcademicoBackendDto>> CrearPlanAsync(PlanAcademicoItem p)
        => await _api.PostAsync<PlanAcademicoBackendDto>("PlanAcademico", new
        {
            Nombre = p.Nombre,
            Programa = string.IsNullOrWhiteSpace(p.Jornada) ? "General" : p.Jornada,
            Anio = DateTime.Now.Year,
            Estado = "Activo"
        });

    public async Task<ApiResponse<string>> AgregarSemestreAsync(int idPlan, int numeroSemestre)
        => await _api.PostAsync($"PlanAcademico/{idPlan}/semestres",
            new { NumeroSemestre = numeroSemestre });

    public async Task<ApiResponse<MateriaPlanBackendDto>> AgregarMateriaAsync(int idSemestrePlan, int idMateria)
        => await _api.PostAsync<MateriaPlanBackendDto>(
            $"PlanAcademico/semestres/{idSemestrePlan}/materias",
            new { IdMateria = idMateria });

    public async Task<ApiResponse<string>> EliminarMateriaAsync(int idMateriaPlan)
        => await _api.DeleteAsync<string>($"PlanAcademico/materias/{idMateriaPlan}");

    public async Task<ApiResponse<string>> ActualizarPlanAsync(PlanAcademicoItem p)
        => await _api.PutAsync($"PlanAcademico/{p.IdPlanAcademico}", new
        {
            Nombre = p.Nombre,
            Programa = string.IsNullOrWhiteSpace(p.Jornada) ? "General" : p.Jornada,
            Anio = DateTime.Now.Year,
            Estado = "Activo"
        });

    public async Task<ApiResponse<int>> EliminarPlanAsync(int id)
        => await _api.DeleteAsync<int>($"PlanAcademico/{id}");

    public async Task<ApiResponse<int>> ActivarPlanAsync(int id)
        => await _api.PatchAsync<int>($"PlanAcademico/{id}/activar", new { });

    private static PlanAcademicoItem MapearItem(PlanAcademicoBackendDto p) => new()
{
    IdPlanAcademico = p.IdPlanAcademico,
    Nombre = p.Nombre,
    Jornada = p.Programa,
    Estado = string.IsNullOrWhiteSpace(p.Estado) ? "Activo" : p.Estado,
    CargaPorSemestre = "Por definir",
    TotalSemestres = p.Semestres?.Count ?? 0,
    TotalMaterias = p.Semestres?.Sum(s => s.Materias?.Count ?? 0) ?? 0,
    TotalCreditos = p.Semestres?.Sum(s => s.Materias?.Sum(m => m.Creditos) ?? 0) ?? 0,
    Semestres = new ObservableCollection<SemestrePlanItem>(
        (p.Semestres ?? new()).Select(s => new SemestrePlanItem
        {
            IdSemestre = s.IdSemestrePlan,
            NumeroSemestre = s.NumeroSemestre,
            Materias = new ObservableCollection<MateriaItem>(
                (s.Materias ?? new()).Select(m => new MateriaItem
                {
                    IdMateria = m.IdMateria,
                    IdMateriaPlan = m.IdMateriaPlan,
                    Codigo = m.Codigo,
                    Nombre = m.Nombre,
                    Creditos = m.Creditos,
                    IntensidadHorariaSemanal = m.IntensidadHorariaSemanal,
                    Activa = true
                })
            )
        })
    )
};
}


