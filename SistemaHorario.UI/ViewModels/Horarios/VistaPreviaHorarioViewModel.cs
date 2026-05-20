using SistemaHorario.UI.Models.UI;
using SistemaHorario.UI.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace SistemaHorario.UI.ViewModels.Horarios
{
    public class VistaPreviaHorarioViewModel
    {
        private readonly HorariosApiService _api = new();

        public HorarioItem Horario { get; set; }
        public bool ModoEdicion { get; set; }
        public ObservableCollection<BloqueHorarioItem> Bloques { get; set; }
        public Dictionary<string, int> FranjasLookup { get; private set; } = new();

        public VistaPreviaHorarioViewModel(HorarioItem horario, bool modoEdicion)
        {
            Horario = horario;
            ModoEdicion = modoEdicion;
            Bloques = new ObservableCollection<BloqueHorarioItem>();
        }

        public async Task CargarBloquesAsync()
        {
            if (Horario.IdGrupo <= 0) return;

            var resp = await _api.ObtenerBloquesPorGrupoAsync(Horario.IdGrupo);
            if (resp.Success && resp.Data != null)
            {
                Bloques.Clear();
                foreach (var bloque in resp.Data)
                    Bloques.Add(bloque);
            }
        }

        public async Task CargarFranjasAsync()
        {
            FranjasLookup = await _api.ObtenerFranjasLookupAsync();
        }

        public async Task<bool> GuardarBloqueAsync(BloqueHorarioItem bloque)
        {
            string key = $"{bloque.Dia}_{bloque.HoraInicio}";
            if (!FranjasLookup.TryGetValue(key, out int idFranja))
                return false;

            bloque.IdFranjaHoraria = idFranja;
            var resp = await _api.ActualizarBloqueAsync(
                bloque.IdHorario, bloque.IdMateria, bloque.IdDocente, idFranja);
            return resp.Success;
        }
    }
}
