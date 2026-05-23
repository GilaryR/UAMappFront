using SistemaHorario.UI.Models.UI;
using SistemaHorario.UI.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaHorario.UI.ViewModels.Docentes
{
    public class DocentesViewModel
    {
        private readonly DocentesApiService _api = new();

        private List<DocenteItem> _docentesBase = new();

        public List<DocenteItem> Docentes { get; private set; } = new();

        public ResumenDocenteItem Resumen { get; private set; } = new();

        public string MensajeEstado { get; private set; } = string.Empty;

        public async Task CargarDatosAsync()
        {
            var resp = await _api.ObtenerDocentesAsync();

            if (!resp.Success || resp.Data == null)
            {
                MensajeEstado = "Error al cargar docentes: " + resp.Message;
                return;
            }

            _docentesBase = resp.Data;
            Docentes = _docentesBase.ToList();

            MensajeEstado = string.Empty;

            CalcularResumen();
        }

        public void Filtrar(
            string textoBusqueda,
            string estado)
        {
            IEnumerable<DocenteItem> consulta = _docentesBase;

            if (!string.IsNullOrWhiteSpace(textoBusqueda))
            {
                string texto = textoBusqueda.Trim().ToLower();

                consulta = consulta.Where(d =>
                    d.NombreCompleto.ToLower().Contains(texto) ||
                    d.Identificacion.ToLower().Contains(texto) ||
                    d.CorreoInstitucional.ToLower().Contains(texto));
            }

            if (!string.IsNullOrWhiteSpace(estado) &&
                estado != "Todos")
            {
                consulta = consulta.Where(d =>
                    d.Estado == estado);
            }

            Docentes = consulta.ToList();

            CalcularResumen();
        }

        public async Task<bool> AgregarDocenteAsync(
            DocenteItem docente)
        {
            var resp = await _api.CrearDocenteAsync(docente);

            if (!resp.Success)
            {
                MensajeEstado = resp.Message;
                return false;
            }

            await CargarDatosAsync();

            return true;
        }

        public async Task<bool> ActualizarDocenteAsync(
            DocenteItem docente)
        {
            var resp = await _api.ActualizarDocenteAsync(docente);

            if (!resp.Success)
            {
                MensajeEstado = resp.Message;
                return false;
            }

            await CargarDatosAsync();

            return true;
        }

        public async Task<bool> InactivarDocenteAsync(
            int idDocente)
        {
            var resp = await _api.InactivarDocenteAsync(idDocente);

            if (!resp.Success)
            {
                MensajeEstado = resp.Message;
                return false;
            }

            await CargarDatosAsync();

            return true;
        }

        public async Task<bool> ActivarDocenteAsync(
            int idDocente)
        {
            var resp = await _api.ActivarDocenteAsync(idDocente);

            if (!resp.Success)
            {
                MensajeEstado = resp.Message;
                return false;
            }

            await CargarDatosAsync();

            return true;
        }

        private void CalcularResumen()
        {
            Resumen = new ResumenDocenteItem
            {
                TotalDocentes = Docentes.Count,
                Activos = Docentes.Count(d => d.Estado == "Activo"),
                Inactivos = Docentes.Count(d => d.Estado == "Inactivo"),
                Disponibles = Docentes.Count
            };
        }
    }
}
