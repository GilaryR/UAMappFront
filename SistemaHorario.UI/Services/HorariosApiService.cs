using SistemaHorario.UI.Models.UI;
using SistemaHorarios.Application.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        ApiResponse<List<HorarioBackendDto>> resp =
            await _api.GetAsync<List<HorarioBackendDto>>("horarios");

        if (!resp.Success || resp.Data == null)
        {
            return new ApiResponse<List<HorarioItem>>
            {
                Success = false,
                Message = resp.Message,
                Data = new List<HorarioItem>()
            };
        }

        List<HorarioItem> lista =
            resp.Data.Select(MapearHorario).ToList();

        return new ApiResponse<List<HorarioItem>>
        {
            Success = true,
            Message = "Horarios obtenidos correctamente.",
            Data = lista
        };
    }

    public async Task<ApiResponse<List<BloqueHorarioItem>>> ObtenerBloquesPorGrupoAsync(
        int idGrupo)
    {
        ApiResponse<List<HorarioBackendDto>> resp =
            await _api.GetAsync<List<HorarioBackendDto>>(
                $"horarios/grupo/{idGrupo}"
            );

        if (!resp.Success || resp.Data == null)
        {
            return new ApiResponse<List<BloqueHorarioItem>>
            {
                Success = false,
                Message = resp.Message,
                Data = new List<BloqueHorarioItem>()
            };
        }

        List<BloqueHorarioItem> bloques =
            resp.Data.Select(MapearBloque).ToList();

        return new ApiResponse<List<BloqueHorarioItem>>
        {
            Success = true,
            Message = "Bloques obtenidos correctamente.",
            Data = bloques
        };
    }

    public async Task<ApiResponse<string>> GenerarHorariosAsync(int idGrupo)
    {
        return await _api.PostAsync(
            $"horarios/generar/{idGrupo}",
            new { }
        );
    }

    public async Task<ApiResponse<string>> EliminarHorarioAsync(int idHorario)
    {
        return await _api.DeleteAsync($"horarios/{idHorario}");
    }

    public async Task<ApiResponse<string>> AprobarHorarioAsync(int idHorario)
    {
        return await _api.PostAsync(
            $"horarios/{idHorario}/aprobar",
            new { }
        );
    }

    public async Task<ApiResponse<string>> RechazarHorarioAsync(int idHorario)
    {
        return await _api.PostAsync(
            $"horarios/{idHorario}/rechazar",
            new { }
        );
    }

    public async Task<ApiResponse<string>> ActualizarBloqueAsync(
        int idHorario,
        int idMateria,
        int idDocente,
        int idFranjaHoraria)
    {
        return await _api.PutAsync($"horarios/{idHorario}/asignatura", new
        {
            IdMateria = idMateria,
            IdDocente = idDocente,
            IdFranjaHoraria = idFranjaHoraria,
            Observacion = string.Empty
        });
    }

    public async Task<Dictionary<string, int>> ObtenerFranjasLookupAsync()
    {
        ApiResponse<List<FranjaSimpleDto>> resp =
            await _api.GetAsync<List<FranjaSimpleDto>>("franjas-horarias");

        Dictionary<string, int> lookup =
            new(StringComparer.OrdinalIgnoreCase);

        if (!resp.Success || resp.Data == null)
        {
            return lookup;
        }

        foreach (FranjaSimpleDto franja in resp.Data)
        {
            string horaKey = TimeSpan.TryParse(
                franja.HoraInicio,
                out TimeSpan horaInicio)
                    ? FormatearHora(horaInicio)
                    : franja.HoraInicio;

            lookup[$"{franja.DiaSemana}_{horaKey}"] =
                franja.IdFranjaHoraria;
        }

        return lookup;
    }

    private static HorarioItem MapearHorario(HorarioBackendDto dto)
    {
        return new HorarioItem
        {
            IdHorario = dto.IdHorario,
            IdGrupo = dto.IdGrupo,
            Nombre = $"{dto.NombreMateria} – {dto.NombreDocente}",
            Grupo = dto.NombreGrupo,
            Tipo = dto.TipoGrupo,
            Jornada = dto.Jornada,
            FechaGeneracion = dto.HorarioTexto,
            Estado = string.IsNullOrWhiteSpace(dto.EstadoTexto)
                ? (dto.Activo ? "Activo" : "Inactivo")
                : dto.EstadoTexto
        };
    }

    private static BloqueHorarioItem MapearBloque(HorarioBackendDto dto)
    {
        return new BloqueHorarioItem
        {
            IdHorario = dto.IdHorario,
            IdMateria = dto.IdMateria,
            IdDocente = dto.IdDocente,
            IdFranjaHoraria = dto.IdFranjaHoraria,
            Dia = dto.DiaSemana,
            HoraInicio = FormatearHora(dto.HoraInicio),
            HoraFinal = FormatearHora(dto.HoraFin),
            Materia = dto.NombreMateria,
            Docente = dto.NombreDocente,
            Aula = string.Empty,
            Modalidad = string.Empty,
            ColorVisual = GenerarColor(dto.NombreMateria)
        };
    }

    private static string FormatearHora(TimeSpan hora)
    {
        int hora12 = hora.Hours > 12 ? hora.Hours - 12 : hora.Hours;

        return $"{hora12}:{hora.Minutes:D2}";
    }

    private static readonly string[] _colores =
    {
        "#20A848",
        "#C77EE8",
        "#0FB8C8",
        "#F3D98B",
        "#F58B8B",
        "#4A90D9",
        "#FF9F40",
        "#E06C75"
    };

    private static string GenerarColor(string materia)
    {
        return _colores[
            Math.Abs(materia.GetHashCode()) % _colores.Length
        ];
    }

    private class FranjaSimpleDto
    {
        public int IdFranjaHoraria { get; set; }
        public string DiaSemana { get; set; } = string.Empty;
        public string HoraInicio { get; set; } = string.Empty;
    }
}