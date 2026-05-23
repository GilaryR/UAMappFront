using SistemaHorario.UI.Models.UI;
using SistemaHorarios.Application.Common;

namespace SistemaHorario.UI.Services;

public class MateriasApiService
{
    private readonly ApiClient _api = new();

    public async Task<ApiResponse<List<MateriaItem>>> ObtenerMateriasAsync()
        => await _api.GetAsync<List<MateriaItem>>("materias");

    public async Task<ApiResponse<string>> CrearMateriaAsync(MateriaItem materia)
        => await _api.PostAsync("materias", new
        {
            materia.Codigo,
            materia.Nombre,
            materia.Creditos,
            materia.IntensidadHorariaSemanal,
            materia.Semestre,
            materia.CantidadGrupos
        });

    public async Task<ApiResponse<string>> ActualizarMateriaAsync(MateriaItem materia)
        => await _api.PutAsync($"materias/{materia.IdMateria}", new
        {
            materia.Codigo,
            materia.Nombre,
            materia.Creditos,
            materia.IntensidadHorariaSemanal,
            materia.Semestre,
            materia.CantidadGrupos,
            materia.Activa
        });

    public async Task<ApiResponse<int>> InactivarMateriaAsync(int idMateria)
        => await _api.DeleteAsync<int>($"materias/{idMateria}");

    public async Task<ApiResponse<int>> ActivarMateriaAsync(int idMateria)
        => await _api.PatchAsync<int>($"materias/{idMateria}/activar", new { });
}