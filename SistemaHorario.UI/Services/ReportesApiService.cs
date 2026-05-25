using SistemaHorarios.Application.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace SistemaHorario.UI.Services;

public class ReporteTipoBackendDto
{
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
}

public class ReporteHorarioGeneradoBackendDto
{
    public int IdGrupo { get; set; }
    public string CodigoGrupo { get; set; } = string.Empty;
    public string NombreGrupo { get; set; } = string.Empty;
    public string Jornada { get; set; } = string.Empty;
    public int NumeroSemestre { get; set; }
    public int TotalMaterias { get; set; }
    public int TotalDocentes { get; set; }
    public int TotalBloques { get; set; }
    public int TotalHoras { get; set; }
    public string Estado { get; set; } = string.Empty;
}

public class ReporteHorarioGrupoBackendDto
{
    public int IdHorario { get; set; }
    public int IdGrupo { get; set; }
    public string CodigoGrupo { get; set; } = string.Empty;
    public string NombreGrupo { get; set; } = string.Empty;
    public string Jornada { get; set; } = string.Empty;
    public int NumeroSemestre { get; set; }
    public string CodigoMateria { get; set; } = string.Empty;
    public string Materia { get; set; } = string.Empty;
    public string Docente { get; set; } = string.Empty;
    public string DiaSemana { get; set; } = string.Empty;
    public string HoraInicio { get; set; } = string.Empty;
    public string HoraFin { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
}

public class ReporteHorarioDocenteBackendDto
{
    public int IdHorario { get; set; }
    public int IdDocente { get; set; }
    public string Docente { get; set; } = string.Empty;
    public string CodigoGrupo { get; set; } = string.Empty;
    public string NombreGrupo { get; set; } = string.Empty;
    public string Jornada { get; set; } = string.Empty;
    public int NumeroSemestre { get; set; }
    public string CodigoMateria { get; set; } = string.Empty;
    public string Materia { get; set; } = string.Empty;
    public string DiaSemana { get; set; } = string.Empty;
    public string HoraInicio { get; set; } = string.Empty;
    public string HoraFin { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
}

public class ReporteCargaDocenteBackendDto
{
    public int IdDocente { get; set; }
    public string Docente { get; set; } = string.Empty;
    public string EstadoDocente { get; set; } = string.Empty;
    public int CantidadMaterias { get; set; }
    public int CantidadGrupos { get; set; }
    public int BloquesSemanales { get; set; }
    public int HorasSemanales { get; set; }
}

public class ReporteConflictoHorarioBackendDto
{
    public string TipoConflicto { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public string Grupo { get; set; } = string.Empty;
    public string Docente { get; set; } = string.Empty;
    public string Materia { get; set; } = string.Empty;
    public string DiaSemana { get; set; } = string.Empty;
    public string HoraInicio { get; set; } = string.Empty;
    public string HoraFin { get; set; } = string.Empty;
}

public class ReportesApiService
{
    private readonly ApiClient _api = new();

    public async Task<ApiResponse<List<ReporteTipoBackendDto>>> ObtenerTiposReporteCompletosAsync()
        => await _api.GetAsync<List<ReporteTipoBackendDto>>("catalogos/tipos-reporte");

    public async Task<ApiResponse<List<string>>> ObtenerTiposReporteAsync()
    {
        var resp = await ObtenerTiposReporteCompletosAsync();

        if (!resp.Success || resp.Data == null)
        {
            return new ApiResponse<List<string>>
            {
                Success = false,
                Message = resp.Message
            };
        }

        return new ApiResponse<List<string>>
        {
            Success = true,
            Data = resp.Data.Select(item => item.Nombre).ToList()
        };
    }

    public async Task<ApiResponse<List<string>>> ObtenerFormatosReporteAsync()
        => await _api.GetAsync<List<string>>("catalogos/formatos-reporte");

    public async Task<ApiResponse<List<int>>> ObtenerSemestresAsync(string? jornada = null)
    {
        string ruta = "catalogos/semestres";

        if (!string.IsNullOrWhiteSpace(jornada))
        {
            ruta += $"?jornada={Uri.EscapeDataString(jornada)}";
        }

        return await _api.GetAsync<List<int>>(ruta);
    }

    public async Task<ApiResponse<List<ReporteHorarioGeneradoBackendDto>>> ObtenerHorariosGeneradosAsync()
        => await _api.GetAsync<List<ReporteHorarioGeneradoBackendDto>>("reportes/horarios-generados");

    public async Task<ApiResponse<List<ReporteHorarioGrupoBackendDto>>> ObtenerHorarioGrupoAsync(int idGrupo)
        => await _api.GetAsync<List<ReporteHorarioGrupoBackendDto>>($"reportes/horario-grupo/{idGrupo}");

    public async Task<ApiResponse<List<ReporteHorarioDocenteBackendDto>>> ObtenerHorarioDocenteAsync(int idDocente)
        => await _api.GetAsync<List<ReporteHorarioDocenteBackendDto>>($"reportes/horario-docente/{idDocente}");

    public async Task<ApiResponse<List<ReporteCargaDocenteBackendDto>>> ObtenerCargaDocenteAsync()
        => await _api.GetAsync<List<ReporteCargaDocenteBackendDto>>("reportes/carga-docente");

    public async Task<ApiResponse<List<ReporteConflictoHorarioBackendDto>>> ObtenerConflictosHorarioAsync()
        => await _api.GetAsync<List<ReporteConflictoHorarioBackendDto>>("reportes/conflictos-horarios");

    public async Task<ApiResponse<string>> DescargarReporteAsync(
        string tipo,
        string formato,
        int? idGrupo,
        int? idDocente,
        string rutaDestino)
    {
        try
        {
            using HttpClient cliente = new()
            {
                BaseAddress = new Uri(ObtenerBaseUrl())
            };

            string ruta =
                $"reportes/descargar?tipo={Uri.EscapeDataString(tipo)}" +
                $"&formato={Uri.EscapeDataString(formato)}";

            if (idGrupo.HasValue)
            {
                ruta += $"&idGrupo={idGrupo.Value}";
            }

            if (idDocente.HasValue)
            {
                ruta += $"&idDocente={idDocente.Value}";
            }

            HttpResponseMessage response = await cliente.GetAsync(ruta);

            if (!response.IsSuccessStatusCode)
            {
                string mensaje = await response.Content.ReadAsStringAsync();

                return new ApiResponse<string>
                {
                    Success = false,
                    Message = string.IsNullOrWhiteSpace(mensaje)
                        ? "No se pudo descargar el reporte."
                        : mensaje
                };
            }

            byte[] contenido = await response.Content.ReadAsByteArrayAsync();
            await File.WriteAllBytesAsync(rutaDestino, contenido);

            return new ApiResponse<string>
            {
                Success = true,
                Message = "Reporte descargado correctamente.",
                Data = rutaDestino
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<string>
            {
                Success = false,
                Message = ex.Message
            };
        }
    }

    private static string ObtenerBaseUrl()
    {
        const string baseUrlPorDefecto = "http://localhost:5023/api/";

        string rutaConfiguracion = Path.Combine(AppContext.BaseDirectory, "appsettings.json");

        if (!File.Exists(rutaConfiguracion))
        {
            return baseUrlPorDefecto;
        }

        try
        {
            string contenido = File.ReadAllText(rutaConfiguracion);
            using JsonDocument documento = JsonDocument.Parse(contenido);

            if (!documento.RootElement.TryGetProperty("ApiSettings", out JsonElement apiSettings))
            {
                return baseUrlPorDefecto;
            }

            if (!apiSettings.TryGetProperty("BaseUrl", out JsonElement baseUrlElemento))
            {
                return baseUrlPorDefecto;
            }

            string? baseUrl = baseUrlElemento.GetString();

            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                return baseUrlPorDefecto;
            }

            return baseUrl.EndsWith('/') ? baseUrl : baseUrl + "/";
        }
        catch
        {
            return baseUrlPorDefecto;
        }
    }
}
