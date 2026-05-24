using SistemaHorario.UI.Models.UI;
using SistemaHorarios.Application.Common;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

    public bool Activo { get; set; }

    public string EstadoTexto { get; set; } = string.Empty;
}

public class GruposApiService
{
    private readonly ApiClient _api = new();

    private class PlanSimpleDto
    {
        public int IdPlanAcademico { get; set; }
    }

    private async Task<int> ObtenerPrimerPlanIdAsync()
    {
        var resp =
            await _api.GetAsync<List<PlanSimpleDto>>("PlanAcademico");

        return resp.Success && resp.Data?.Count > 0
            ? resp.Data[0].IdPlanAcademico
            : 0;
    }

    public async Task<ApiResponse<List<GrupoAcademicoItem>>> ObtenerGruposAsync()
    {
        var resp =
            await _api.GetAsync<List<GrupoBackendDto>>("Grupos");

        if (!resp.Success || resp.Data == null)
        {
            return new ApiResponse<List<GrupoAcademicoItem>>
            {
                Success = false,
                Message = resp.Message
            };
        }

        var lista = resp.Data
            .Select(g => new GrupoAcademicoItem
            {
                IdGrupoAcademico = g.IdGrupo,
                IdPlanAcademico = g.IdPlanAcademico,
                NumeroSemestre = g.NumeroSemestre,
                Codigo = g.Codigo,
                NombreGrupo = g.Nombre,
                Jornada = g.Jornada,
                Tipo = g.TipoGrupo,
                Estado = g.Activo ? "Activo" : "Inactivo",
                EstudiantesActuales = g.CantidadEstudiantes,
                PlazasDisponibles = g.CantidadEstudiantes
            })
            .ToList();

        return new ApiResponse<List<GrupoAcademicoItem>>
        {
            Success = true,
            Data = lista
        };
    }

    public async Task<ApiResponse<string>> CrearGrupoAsync(
        GrupoAcademicoItem grupo)
    {
        int idPlan =
            grupo.IdPlanAcademico > 0
                ? grupo.IdPlanAcademico
                : await ObtenerPrimerPlanIdAsync();

        int semestre =
            grupo.NumeroSemestre > 0
                ? grupo.NumeroSemestre
                : 1;

        return await _api.PostAsync("Grupos", new
        {
            grupo.Codigo,
            Nombre = grupo.NombreGrupo,
            grupo.Jornada,
            TipoGrupo = grupo.Tipo,
            NumeroSemestre = semestre,
            CantidadEstudiantes = grupo.PlazasDisponibles,
            IdPlanAcademico = idPlan
        });
    }

    public async Task<ApiResponse<string>> ActualizarGrupoAsync(
        GrupoAcademicoItem grupo)
    {
        int idPlan =
            grupo.IdPlanAcademico > 0
                ? grupo.IdPlanAcademico
                : await ObtenerPrimerPlanIdAsync();

        int semestre =
            grupo.NumeroSemestre > 0
                ? grupo.NumeroSemestre
                : 1;

        return await _api.PutAsync($"Grupos/{grupo.IdGrupoAcademico}", new
        {
            grupo.Codigo,
            Nombre = grupo.NombreGrupo,
            grupo.Jornada,
            TipoGrupo = grupo.Tipo,
            NumeroSemestre = semestre,
            CantidadEstudiantes = grupo.PlazasDisponibles,
            IdPlanAcademico = idPlan,
            Activo = grupo.Estado == "Activo"
        });
    }

    public async Task<ApiResponse<int>> EliminarGrupoAsync(int id)
    {
        return await _api.DeleteAsync<int>($"Grupos/{id}");
    }

    public async Task<ApiResponse<int>> ActivarGrupoAsync(int id)
    {
        return await _api.PatchAsync<int>($"Grupos/{id}/activar", new { });
    }

    public async Task<ApiResponse<List<GrupoHorarioOption>>> ObtenerGruposParaHorarioAsync()
    {
        var resp =
            await _api.GetAsync<List<GrupoBackendDto>>("Grupos");

        if (!resp.Success || resp.Data == null)
        {
            return new ApiResponse<List<GrupoHorarioOption>>
            {
                Success = false,
                Message = resp.Message
            };
        }

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

        return new ApiResponse<List<GrupoHorarioOption>>
        {
            Success = true,
            Data = lista
        };
    }
}