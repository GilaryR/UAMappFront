using SistemaHorario.UI.Models.UI;
using SistemaHorario.UI.Services;
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
    }
}
