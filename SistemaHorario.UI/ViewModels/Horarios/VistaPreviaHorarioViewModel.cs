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
        public string MensajeEstado { get; private set; } = string.Empty;

        public VistaPreviaHorarioViewModel(HorarioItem horario, bool modoEdicion)
        {
            Horario = horario;
            ModoEdicion = modoEdicion;
            Bloques = new ObservableCollection<BloqueHorarioItem>();
        }

        public async Task CargarBloquesAsync()
        {
            Bloques.Clear();

            if (Horario.EsHorarioDocente)
            {
                await CargarBloquesDocenteAsync();
                return;
            }

            await CargarBloquesGrupoAsync();
        }

        private async Task CargarBloquesGrupoAsync()
        {
            if (Horario.IdGrupo <= 0)
            {
                MensajeEstado = "No se encontró el grupo asociado al horario.";
                return;
            }

            var resp = await _api.ObtenerBloquesPorGrupoAsync(Horario.IdGrupo);
            if (!resp.Success || resp.Data == null)
            {
                MensajeEstado = string.IsNullOrWhiteSpace(resp.Message)
                    ? "No se pudieron cargar los bloques del grupo."
                    : resp.Message;
                return;
            }

            foreach (BloqueHorarioItem bloque in resp.Data)
            {
                Bloques.Add(bloque);
            }

            MensajeEstado = string.Empty;
        }

        private async Task CargarBloquesDocenteAsync()
        {
            if (Horario.IdDocente <= 0)
            {
                MensajeEstado = "No se encontró el docente asociado al horario.";
                return;
            }

            var resp = await _api.ObtenerBloquesPorDocenteAsync(Horario.IdDocente);
            if (!resp.Success || resp.Data == null)
            {
                MensajeEstado = string.IsNullOrWhiteSpace(resp.Message)
                    ? "No se pudo cargar el horario del docente."
                    : resp.Message;
                return;
            }

            foreach (BloqueHorarioItem bloque in resp.Data)
            {
                Bloques.Add(bloque);
            }

            MensajeEstado = string.Empty;
        }

        public async Task CargarFranjasAsync()
        {
            FranjasLookup = await _api.ObtenerFranjasLookupAsync();
        }

        public async Task<bool> GuardarBloqueAsync(BloqueHorarioItem bloque)
        {
            string key = $"{bloque.Dia}_{bloque.HoraInicio}";

            if (!FranjasLookup.TryGetValue(key, out int idFranja))
            {
                MensajeEstado = "No existe una franja activa para el día y la hora seleccionados.";
                return false;
            }

            bloque.IdFranjaHoraria = idFranja;

            var resp = await _api.ActualizarBloqueAsync(
                bloque.IdHorario,
                bloque.IdMateria,
                bloque.IdDocente,
                idFranja);

            if (!resp.Success)
            {
                MensajeEstado = string.IsNullOrWhiteSpace(resp.Message)
                    ? "No se pudo guardar el cambio."
                    : resp.Message;
                return false;
            }

            MensajeEstado = string.Empty;
            return true;
        }
    }
}
