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
    public bool Activo { get; set; }
    public string EstadoTexto { get; set; } = string.Empty;
}

public class GruposApiService
{
    private readonly ApiClient _api = new();

    public async Task<ApiResponse<List<GrupoAcademicoItem>>> ObtenerGruposAsync()
    {
        var resp = await _api.GetAsync<List<GrupoBackendDto>>("Grupos");
        if (!resp.Success || resp.Data == null)
            return new ApiResponse<List<GrupoAcademicoItem>> { Success = false, Message = resp.Message };

        var lista = resp.Data.Where(g => g.Activo).Select(g => new GrupoAcademicoItem
        {
            IdGrupoAcademico = g.IdGrupo,
            Codigo = g.Codigo,
            NombreGrupo = g.Nombre,
            Jornada = g.Jornada,
            Tipo = g.TipoGrupo,
            Materia = string.Empty,
            Estado = g.Activo ? "Activo" : "Inactivo",
            EstudiantesActuales = g.CantidadEstudiantes,
            PlazasDisponibles = g.CantidadEstudiantes,
            Dias = new ObservableCollection<string>()
        }).ToList();

        return new ApiResponse<List<GrupoAcademicoItem>> { Success = true, Data = lista };
    }

    public async Task<ApiResponse<string>> CrearGrupoAsync(GrupoAcademicoItem g)
        => await _api.PostAsync("Grupos", new
        {
            g.Codigo,
            Nombre = g.NombreGrupo,
            g.Jornada,
            TipoGrupo = g.Tipo,
            NumeroSemestre = 1,
            CantidadEstudiantes = g.PlazasDisponibles,
            IdPlanAcademico = 2
        });

    public async Task<ApiResponse<string>> ActualizarGrupoAsync(GrupoAcademicoItem g)
        => await _api.PutAsync($"Grupos/{g.IdGrupoAcademico}", new
        {
            g.Codigo,
            Nombre = g.NombreGrupo,
            g.Jornada,
            TipoGrupo = g.Tipo,
            NumeroSemestre = 1,
            CantidadEstudiantes = g.PlazasDisponibles,
            IdPlanAcademico = 2,
            Activo = g.Estado == "Activo"
        });

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
