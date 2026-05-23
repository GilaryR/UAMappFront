using SistemaHorario.UI.Models.UI;
using SistemaHorario.UI.Services;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaHorario.UI.ViewModels.Horarios
{
    public class GenerarHorarioViewModel
    {
        private readonly GruposApiService _gruposApi = new();
        private readonly HorariosApiService _horariosApi = new();

        public ObservableCollection<GrupoHorarioOption> Grupos { get; set; }

        public GenerarHorarioRequestUI Request { get; set; }

        public string MensajeEstado { get; private set; } = string.Empty;

        public GenerarHorarioViewModel()
        {
            Request = new GenerarHorarioRequestUI();
            Grupos = new ObservableCollection<GrupoHorarioOption>();
        }

        public async Task CargarGruposAsync()
        {
            var resp = await _gruposApi.ObtenerGruposParaHorarioAsync();

            Grupos.Clear();

            if (!resp.Success || resp.Data == null)
            {
                MensajeEstado = string.IsNullOrWhiteSpace(resp.Message)
                    ? "No se pudieron cargar los grupos disponibles."
                    : resp.Message;

                return;
            }

            foreach (GrupoHorarioOption grupo in resp.Data)
            {
                Grupos.Add(grupo);
            }

            MensajeEstado = string.Empty;
        }

        public async Task<bool> GenerarHorarioAsync(int idGrupo)
        {
            var resp = await _horariosApi.GenerarHorariosAsync(idGrupo);

            if (!resp.Success)
            {
                MensajeEstado = string.IsNullOrWhiteSpace(resp.Message)
                    ? "No se pudo generar el horario."
                    : resp.Message;

                return false;
            }

            MensajeEstado = string.Empty;
            return true;
        }

        public async Task<HorarioItem?> ObtenerUltimoHorarioGeneradoAsync(int idGrupo)
        {
            var resp = await _horariosApi.ObtenerHorariosAsync();

            if (!resp.Success || resp.Data == null)
            {
                MensajeEstado = string.IsNullOrWhiteSpace(resp.Message)
                    ? "El horario fue generado, pero no se pudo consultar la lista actual."
                    : resp.Message;

                return null;
            }

            HorarioItem? horarioGenerado =
                resp.Data
                    .Where(h => h.IdGrupo == idGrupo)
                    .OrderByDescending(h => h.IdHorario)
                    .FirstOrDefault();

            if (horarioGenerado == null)
            {
                MensajeEstado =
                    "El horario fue generado, pero no se encontró en la lista actual.";

                return null;
            }

            MensajeEstado = string.Empty;
            return horarioGenerado;
        }
    }
}