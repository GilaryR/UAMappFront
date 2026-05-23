using SistemaHorario.UI.Models.UI;
using SistemaHorarios.Application.Common;

namespace SistemaHorario.UI.Services;

public class DocenteBackendDto
{
    public int IdDocente { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public string Identificacion { get; set; } = string.Empty;
    public string CorreoInstitucional { get; set; } = string.Empty;
    public bool Activo { get; set; }
    public List<string> Materias { get; set; } = new();
}

public class DisponibilidadBackendDto
{
    public string Dia { get; set; } = string.Empty;
    public string HoraInicio { get; set; } = string.Empty;
    public string HoraFin { get; set; } = string.Empty;
    public bool Disponible { get; set; }
}

public class DocentesApiService
{
    private readonly ApiClient _api = new();

    public async Task<ApiResponse<List<DocenteItem>>> ObtenerDocentesAsync()
    {
        var resp = await _api.GetAsync<List<DocenteBackendDto>>("Docentes");

        if (!resp.Success || resp.Data == null)
        {
            return new ApiResponse<List<DocenteItem>>
            {
                Success = false,
                Message = resp.Message
            };
        }

        var lista = resp.Data.Select(d => new DocenteItem
        {
            IdDocente = d.IdDocente,
            NombreCompleto = d.NombreCompleto,
            Identificacion = d.Identificacion,
            CorreoInstitucional = d.CorreoInstitucional,
            Materias = string.Join(", ", d.Materias),
            Estado = d.Activo ? "Activo" : "Inactivo"
        }).ToList();

        return new ApiResponse<List<DocenteItem>>
        {
            Success = true,
            Data = lista
        };
    }

    public async Task<ApiResponse<DocenteBackendDto>> CrearDocenteAsync(
        DocenteItem docente)
    {
        return await _api.PostAsync<DocenteBackendDto>("Docentes", new
        {
            docente.NombreCompleto,
            docente.Identificacion,
            docente.CorreoInstitucional,
            Activo = docente.Estado == "Activo",
            IdsMateria = docente.IdsMateria
        });
    }

    public async Task<ApiResponse<int>> ActualizarDocenteAsync(
        DocenteItem docente)
    {
        return await _api.PutAsync<int>($"Docentes/{docente.IdDocente}", new
        {
            docente.NombreCompleto,
            docente.Identificacion,
            docente.CorreoInstitucional,
            Activo = docente.Estado == "Activo",
            IdsMateria = docente.IdsMateria
        });
    }

    public async Task<ApiResponse<int>> InactivarDocenteAsync(int id)
    {
        return await _api.DeleteAsync<int>($"Docentes/{id}");
    }

    public async Task<ApiResponse<int>> ActivarDocenteAsync(int id)
    {
        return await _api.PatchAsync<int>($"Docentes/{id}/activar", new { });
    }

    public async Task<ApiResponse<List<DisponibilidadDocenteItem>>> ObtenerDisponibilidadAsync(
        int idDocente)
    {
        var resp =
            await _api.GetAsync<List<DisponibilidadBackendDto>>(
                $"Docentes/{idDocente}/disponibilidad");

        if (!resp.Success || resp.Data == null)
        {
            return new ApiResponse<List<DisponibilidadDocenteItem>>
            {
                Success = false,
                Message = resp.Message
            };
        }

        var lista = resp.Data.Select(d => new DisponibilidadDocenteItem
        {
            Dia = d.Dia,
            HoraInicio = d.HoraInicio,
            HoraFin = d.HoraFin,
            Disponible = d.Disponible
        }).ToList();

        return new ApiResponse<List<DisponibilidadDocenteItem>>
        {
            Success = true,
            Data = lista
        };
    }

    public async Task<ApiResponse<int>> ActualizarDisponibilidadAsync(
        int idDocente,
        List<DisponibilidadDocenteItem> disponibilidad)
    {
        return await _api.PutAsync<int>($"Docentes/{idDocente}/disponibilidad", new
        {
            Disponibilidades = disponibilidad.Select(d => new
            {
                d.Dia,
                d.HoraInicio,
                d.HoraFin,
                d.Disponible
            }).ToList()
        });
    }
}