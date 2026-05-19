using SistemaHorario.UI.Models.UI;
using SistemaHorario.UI.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace SistemaHorario.UI.ViewModels.Horarios
{
    public class GenerarHorarioViewModel
    {
        private readonly GruposApiService _gruposApi = new();

        public ObservableCollection<GrupoHorarioOption> Grupos { get; set; }

        public GenerarHorarioRequestUI Request { get; set; }

        public GenerarHorarioViewModel()
        {
            Request = new GenerarHorarioRequestUI();
            Grupos = new ObservableCollection<GrupoHorarioOption>();
        }

        public async Task CargarGruposAsync()
        {
            var resp = await _gruposApi.ObtenerGruposParaHorarioAsync();
            Grupos.Clear();

            if (resp.Success && resp.Data != null)
            {
                foreach (var grupo in resp.Data)
                    Grupos.Add(grupo);
            }
        }
    }
}
