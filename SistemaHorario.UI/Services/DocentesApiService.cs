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


public class DocentesApiService
{
    private readonly ApiClient _api = new();

    public async Task<ApiResponse<List<DocenteItem>>> ObtenerDocentesAsync()
    {
        var resp = await _api.GetAsync<List<DocenteBackendDto>>("Docentes");
        if (!resp.Success || resp.Data == null)
            return new ApiResponse<List<DocenteItem>> { Success = false, Message = resp.Message };

        var lista = resp.Data.Select(d => new DocenteItem
        {
            IdDocente = d.IdDocente,
            NombreCompleto = d.NombreCompleto,
            Identificacion = d.Identificacion,
            CorreoInstitucional = d.CorreoInstitucional,
            Materias = string.Join(", ", d.Materias),
            Estado = d.Activo ? "Activo" : "Inactivo"
        }).ToList();

        return new ApiResponse<List<DocenteItem>> { Success = true, Data = lista };
    }

    public async Task<ApiResponse<string>> CrearDocenteAsync(DocenteItem d)
        => await _api.PostAsync("Docentes", new
        {
            d.NombreCompleto,
            d.Identificacion,
            d.CorreoInstitucional,
            Activo = d.Estado == "Activo",
            IdsMateria = d.IdsMateria
        });

    public async Task<ApiResponse<string>> ActualizarDocenteAsync(DocenteItem d)
        => await _api.PutAsync($"Docentes/{d.IdDocente}", new
        {
            d.NombreCompleto,
            d.Identificacion,
            d.CorreoInstitucional,
            Activo = d.Estado == "Activo",
            IdsMateria = d.IdsMateria
        });

    public async Task<ApiResponse<string>> EliminarDocenteAsync(int id)
        => await _api.DeleteAsync($"Docentes/{id}");
}
