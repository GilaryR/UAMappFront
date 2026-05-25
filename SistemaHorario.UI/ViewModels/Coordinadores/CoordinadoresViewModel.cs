using SistemaHorario.UI.Models.UI;
using SistemaHorario.UI.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaHorario.UI.ViewModels.Coordinadores
{
    public class CoordinadoresViewModel
    {
        private readonly CoordinadoresApiService _api = new();
        private List<CoordinadorItem> _coordinadoresBase = new();

        public List<CoordinadorItem> Coordinadores { get; private set; } = new();

        public ResumenCoordinadorItem Resumen { get; private set; } = new();

        public string MensajeEstado { get; private set; } = string.Empty;

        public async Task CargarDatosAsync()
        {
            var resp = await _api.ObtenerCoordinadoresAsync();

            if (!resp.Success || resp.Data == null)
            {
                MensajeEstado = string.IsNullOrWhiteSpace(resp.Message)
                    ? "No se pudieron cargar los coordinadores."
                    : resp.Message;

                _coordinadoresBase = new List<CoordinadorItem>();
                Coordinadores = new List<CoordinadorItem>();
                CalcularResumen();

                return;
            }

            _coordinadoresBase = resp.Data;
            Coordinadores = _coordinadoresBase.ToList();
            MensajeEstado = string.Empty;
            CalcularResumen();
        }

        public void Filtrar(
            string textoBusqueda,
            string filtroBusqueda,
            string estado)
        {
            IEnumerable<CoordinadorItem> query = _coordinadoresBase;

            if (!string.IsNullOrWhiteSpace(textoBusqueda))
            {
                string texto = textoBusqueda.ToLower();

                switch (filtroBusqueda.ToLower())
                {
                    case "nombre":
                        query = query.Where(x =>
                            x.NombreCompleto.ToLower().Contains(texto));
                        break;

                    case "cedula":
                    case "cédula":
                        query = query.Where(x =>
                            x.Cedula.ToLower().Contains(texto));
                        break;

                    case "correo":
                        query = query.Where(x =>
                            x.CorreoInstitucional.ToLower().Contains(texto));
                        break;

                    default:
                        query = query.Where(x =>
                            x.NombreCompleto.ToLower().Contains(texto) ||
                            x.Cedula.ToLower().Contains(texto) ||
                            x.CorreoInstitucional.ToLower().Contains(texto));
                        break;
                }
            }

            if (!string.IsNullOrWhiteSpace(estado) && estado != "Todos")
            {
                query = query.Where(x => x.Estado == estado);
            }

            Coordinadores = query.ToList();
            CalcularResumen();
        }

        public async Task<bool> CambiarEstadoCoordinadorAsync(
            CoordinadorItem coordinador)
        {
            string nuevoEstado = coordinador.EstaActivo
                ? "Inactivo"
                : "Activo";

            var resp = await _api.CambiarEstadoCoordinadorAsync(
                coordinador.IdCoordinador,
                nuevoEstado);

            if (!resp.Success)
            {
                MensajeEstado = string.IsNullOrWhiteSpace(resp.Message)
                    ? "No se pudo cambiar el estado del coordinador."
                    : resp.Message;

                return false;
            }

            await CargarDatosAsync();
            MensajeEstado = string.Empty;
            return true;
        }

        public async Task<bool> EliminarCoordinadorAsync(CoordinadorItem coordinador)
        {
            // Eliminar en el módulo equivale a inactivar, no borrar físicamente.
            var resp = await _api.InactivarCoordinadorAsync(coordinador.IdCoordinador);

            if (!resp.Success)
            {
                MensajeEstado = string.IsNullOrWhiteSpace(resp.Message)
                    ? "No se pudo inactivar el coordinador."
                    : resp.Message;

                return false;
            }

            await CargarDatosAsync();
            MensajeEstado = string.Empty;
            return true;
        }

        private void CalcularResumen()
        {
            Resumen = new ResumenCoordinadorItem
            {
                TotalCoordinadores = Coordinadores.Count,
                Activos = Coordinadores.Count(x => x.Estado == "Activo"),
                Inactivos = Coordinadores.Count(x => x.Estado == "Inactivo")
            };
        }
    }
}
