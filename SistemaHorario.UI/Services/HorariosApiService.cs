using SistemaHorario.UI.Models.UI;
using SistemaHorarios.Application.Common;

namespace SistemaHorario.UI.Services;

public class HorarioBackendDto
{
    public int IdHorario { get; set; }
    public int IdGrupo { get; set; }
    public string CodigoGrupo { get; set; } = string.Empty;
    public string NombreGrupo { get; set; } = string.Empty;
    public string Jornada { get; set; } = string.Empty;
    public string TipoGrupo { get; set; } = string.Empty;
    public int IdMateria { get; set; }
    public string CodigoMateria { get; set; } = string.Empty;
    public string NombreMateria { get; set; } = string.Empty;
    public int IdDocente { get; set; }
    public string NombreDocente { get; set; } = string.Empty;
    public int IdFranjaHoraria { get; set; }
    public string DiaSemana { get; set; } = string.Empty;
    public TimeSpan HoraInicio { get; set; }
    public TimeSpan HoraFin { get; set; }
    public string HorarioTexto { get; set; } = string.Empty;
    public string Observacion { get; set; } = string.Empty;
    public bool Activo { get; set; }
    public string EstadoTexto { get; set; } = string.Empty;
}

public class HorariosApiService
{
    private readonly ApiClient _api = new();

    public async Task<ApiResponse<List<HorarioItem>>> ObtenerHorariosAsync()
    {
        var resp = await _api.GetAsync<List<HorarioBackendDto>>("horarios");
        if (!resp.Success || resp.Data == null)
            return new ApiResponse<List<HorarioItem>> { Success = false, Message = resp.Message };

        var lista = resp.Data.Select(MapearHorario).ToList();
        return new ApiResponse<List<HorarioItem>> { Success = true, Data = lista };
    }

    public async Task<ApiResponse<List<BloqueHorarioItem>>> ObtenerBloquesPorGrupoAsync(int idGrupo)
    {
        var resp = await _api.GetAsync<List<HorarioBackendDto>>($"horarios/grupo/{idGrupo}");
        if (!resp.Success || resp.Data == null)
            return new ApiResponse<List<BloqueHorarioItem>> { Success = false, Message = resp.Message };

        var bloques = resp.Data.Select(MapearBloque).ToList();
        return new ApiResponse<List<BloqueHorarioItem>> { Success = true, Data = bloques };
    }

    public async Task<ApiResponse<string>> GenerarHorariosAsync(int idGrupo)
        => await _api.PostAsync($"horarios/generar/{idGrupo}", new { });

    public async Task<ApiResponse<string>> EliminarHorarioAsync(int id)
        => await _api.DeleteAsync($"horarios/{id}");

    private HorarioItem MapearHorario(HorarioBackendDto dto) => new HorarioItem
    {
        IdHorario = dto.IdHorario,
        IdGrupo = dto.IdGrupo,
        Nombre = $"{dto.NombreMateria} – {dto.NombreDocente}",
        Grupo = dto.NombreGrupo,
        Tipo = dto.TipoGrupo,
        Jornada = dto.Jornada,
        FechaGeneracion = dto.HorarioTexto,
        Estado = dto.EstadoTexto
    };

    private static BloqueHorarioItem MapearBloque(HorarioBackendDto dto) => new BloqueHorarioItem
    {
        IdHorario = dto.IdHorario,
        IdMateria = dto.IdMateria,
        IdDocente = dto.IdDocente,
        Dia = dto.DiaSemana,
        HoraInicio = FormatearHora(dto.HoraInicio),
        HoraFinal = FormatearHora(dto.HoraFin),
        Materia = dto.NombreMateria,
        Docente = dto.NombreDocente,
        Aula = string.Empty,
        Modalidad = string.Empty,
        ColorVisual = GenerarColor(dto.NombreMateria)
    };

    private static string FormatearHora(TimeSpan t)
    {
        int h = t.Hours > 12 ? t.Hours - 12 : t.Hours;
        return $"{h}:{t.Minutes:D2}";
    }

    private static readonly string[] _colores =
    {
        "#20A848", "#C77EE8", "#0FB8C8", "#F3D98B",
        "#F58B8B", "#4A90D9", "#FF9F40", "#E06C75"
    };

    private static string GenerarColor(string materia)
        => _colores[Math.Abs(materia.GetHashCode()) % _colores.Length];
}
