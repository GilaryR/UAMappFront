using SistemaHorario.UI.Models.UI;
using SistemaHorarios.Application.Common;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

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

public class PlanAcademicoOption
{
    public int IdPlanAcademico { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Programa { get; set; } = string.Empty;
    public List<int> Semestres { get; set; } = new();
    public Dictionary<int, List<string>> MateriasPorSemestre { get; set; } = new();

    public string DisplayTexto => string.IsNullOrWhiteSpace(Programa)
        ? Nombre
        : $"{Nombre} - {Programa}";
}

public class GruposApiService
{
    private readonly ApiClient _api = new();

    public async Task<ApiResponse<List<PlanAcademicoOption>>> ObtenerPlanesParaGrupoAsync()
    {
        var resp = await _api.GetAsync<List<PlanAcademicoBackendDto>>("PlanAcademico");
        if (!resp.Success || resp.Data == null)
            return new ApiResponse<List<PlanAcademicoOption>> { Success = false, Message = resp.Message };

        var planes = resp.Data
            .Select(p => new PlanAcademicoOption
            {
                IdPlanAcademico = p.IdPlanAcademico,
                Nombre = p.Nombre,
                Programa = p.Programa,
                Semestres = (p.Semestres ?? new())
                    .Select(s => s.NumeroSemestre)
                    .OrderBy(s => s)
                    .ToList(),
                MateriasPorSemestre = (p.Semestres ?? new())
                    .ToDictionary(
                        s => s.NumeroSemestre,
                        s => (s.Materias ?? new())
                            .Select(m => m.Nombre)
                            .Where(nombre => !string.IsNullOrWhiteSpace(nombre))
                            .Distinct(StringComparer.OrdinalIgnoreCase)
                            .OrderBy(nombre => nombre)
                            .ToList())
            })
            .ToList();

        return new ApiResponse<List<PlanAcademicoOption>> { Success = true, Data = planes };
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
        ApiResponse<string>? validacion = ValidarPlanYSemestre(g);
        if (validacion != null)
            return validacion;

        return await _api.PostAsync("Grupos", new
        {
            g.Codigo,
            Nombre = g.NombreGrupo,
            g.Jornada,
            TipoGrupo = g.Tipo,
            g.NumeroSemestre,
            CantidadEstudiantes = g.PlazasDisponibles,
            g.IdPlanAcademico,
            Materia = g.Materia,
            Dias = string.Join(",", g.Dias)
        });
    }

    public async Task<ApiResponse<string>> ActualizarGrupoAsync(GrupoAcademicoItem g)
    {
        ApiResponse<string>? validacion = ValidarPlanYSemestre(g);
        if (validacion != null)
            return validacion;

        return await _api.PutAsync($"Grupos/{g.IdGrupoAcademico}", new
        {
            g.Codigo,
            Nombre = g.NombreGrupo,
            g.Jornada,
            TipoGrupo = g.Tipo,
            g.NumeroSemestre,
            CantidadEstudiantes = g.PlazasDisponibles,
            g.IdPlanAcademico,
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
                IdPlanAcademico = g.IdPlanAcademico,
                NombreGrupo = g.Nombre,
                Semestre = g.NumeroSemestre,
                Jornada = g.Jornada
            })
            .ToList();

        return new ApiResponse<List<GrupoHorarioOption>> { Success = true, Data = lista };
    }

    private static ApiResponse<string>? ValidarPlanYSemestre(GrupoAcademicoItem g)
    {
        if (g.IdPlanAcademico <= 0)
        {
            return new ApiResponse<string>
            {
                Success = false,
                Message = "Selecciona el plan académico del grupo."
            };
        }

        if (g.NumeroSemestre <= 0)
        {
            return new ApiResponse<string>
            {
                Success = false,
                Message = "Selecciona el semestre del plan académico para el grupo."
            };
        }

        return null;
    }
}
