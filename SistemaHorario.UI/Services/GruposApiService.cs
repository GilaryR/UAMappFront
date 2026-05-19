using SistemaHorario.UI.Models.UI;
using SistemaHorarios.Application.Common;
using System.Collections.ObjectModel;

namespace SistemaHorario.UI.Services;

public class GrupoBackendDto
{
    public int IdGrupo { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string Jornada { get; set; } = string.Empty;
    public string TipoGrupo { get; set; } = string.Empty;
    public int NumeroSemestre { get; set; }
    public int CantidadEstudiantes { get; set; }
    public int IdPlanAcademico { get; set; }
    public string Materia { get; set; } = string.Empty;
    public string Dias { get; set; } = string.Empty;
    public bool Activo { get; set; }
    public string EstadoTexto { get; set; } = string.Empty;
}

public class GruposApiService
{
    private readonly ApiClient _api = new();

    private class PlanSimpleDto { public int IdPlanAcademico { get; set; } }

    private async Task<int> ObtenerPrimerPlanIdAsync()
    {
        var resp = await _api.GetAsync<List<PlanSimpleDto>>("PlanAcademico");
        return resp.Success && resp.Data?.Count > 0 ? resp.Data[0].IdPlanAcademico : 0;
    }

    public async Task<ApiResponse<List<GrupoAcademicoItem>>> ObtenerGruposAsync()
    {
        var resp = await _api.GetAsync<List<GrupoBackendDto>>("Grupos");
        if (!resp.Success || resp.Data == null)
            return new ApiResponse<List<GrupoAcademicoItem>> { Success = false, Message = resp.Message };

        var lista = resp.Data.Where(g => g.Activo).Select(g => new GrupoAcademicoItem
        {
            IdGrupoAcademico = g.IdGrupo,
            IdPlanAcademico = g.IdPlanAcademico,
            NumeroSemestre = g.NumeroSemestre,
            Codigo = g.Codigo,
            NombreGrupo = g.Nombre,
            Jornada = g.Jornada,
            Tipo = g.TipoGrupo,
            Materia = g.Materia,
            Estado = g.Activo ? "Activo" : "Inactivo",
            EstudiantesActuales = g.CantidadEstudiantes,
            PlazasDisponibles = g.CantidadEstudiantes,
            Dias = string.IsNullOrWhiteSpace(g.Dias)
                ? new ObservableCollection<string>()
                : new ObservableCollection<string>(g.Dias.Split(',', StringSplitOptions.RemoveEmptyEntries).Select(d => d.Trim()))
        }).ToList();

        return new ApiResponse<List<GrupoAcademicoItem>> { Success = true, Data = lista };
    }

    public async Task<ApiResponse<string>> CrearGrupoAsync(GrupoAcademicoItem g)
    {
        int idPlan = g.IdPlanAcademico > 0 ? g.IdPlanAcademico : await ObtenerPrimerPlanIdAsync();
        int semestre = g.NumeroSemestre > 0 ? g.NumeroSemestre : 1;

        return await _api.PostAsync("Grupos", new
        {
            g.Codigo,
            Nombre = g.NombreGrupo,
            g.Jornada,
            TipoGrupo = g.Tipo,
            NumeroSemestre = semestre,
            CantidadEstudiantes = g.PlazasDisponibles,
            IdPlanAcademico = idPlan,
            Materia = g.Materia,
            Dias = string.Join(",", g.Dias)
        });
    }

    public async Task<ApiResponse<string>> ActualizarGrupoAsync(GrupoAcademicoItem g)
    {
        int idPlan = g.IdPlanAcademico > 0 ? g.IdPlanAcademico : await ObtenerPrimerPlanIdAsync();
        int semestre = g.NumeroSemestre > 0 ? g.NumeroSemestre : 1;

        return await _api.PutAsync($"Grupos/{g.IdGrupoAcademico}", new
        {
            g.Codigo,
            Nombre = g.NombreGrupo,
            g.Jornada,
            TipoGrupo = g.Tipo,
            NumeroSemestre = semestre,
            CantidadEstudiantes = g.PlazasDisponibles,
            IdPlanAcademico = idPlan,
            Materia = g.Materia,
            Dias = string.Join(",", g.Dias),
            Activo = g.Estado == "Activo"
        });
    }

    public async Task<ApiResponse<string>> EliminarGrupoAsync(int id)
        => await _api.DeleteAsync($"Grupos/{id}");

    public async Task<ApiResponse<List<GrupoHorarioOption>>> ObtenerGruposParaHorarioAsync()
    {
        var resp = await _api.GetAsync<List<GrupoBackendDto>>("Grupos");
        if (!resp.Success || resp.Data == null)
            return new ApiResponse<List<GrupoHorarioOption>> { Success = false, Message = resp.Message };

        var lista = resp.Data
            .Where(g => g.Activo)
            .Select(g => new GrupoHorarioOption
            {
                IdGrupo = g.IdGrupo,
                NombreGrupo = g.Nombre,
                Semestre = g.NumeroSemestre,
                Jornada = g.Jornada
            })
            .ToList();

        return new ApiResponse<List<GrupoHorarioOption>> { Success = true, Data = lista };
    }
}
