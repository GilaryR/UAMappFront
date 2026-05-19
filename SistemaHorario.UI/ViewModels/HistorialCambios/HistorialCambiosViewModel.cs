using SistemaHorario.UI.Models.UI;
using SistemaHorario.UI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaHorario.UI.ViewModels.HistorialCambios
{
    public class HistorialCambiosViewModel
    {
        private readonly HistorialCambiosApiService _api = new();
        private List<HistorialCambioItem> _historialBase = new();

        public List<HistorialCambioItem> Actividades { get; private set; } = new();
        public List<string> Usuarios { get; private set; } = new();
        public List<string> Modulos { get; private set; } = new();
        public string MensajeEstado { get; private set; } = string.Empty;

        public HistorialCambiosViewModel() { }

        public async Task CargarDatosAsync()
        {
            var resp = await _api.ObtenerHistorialAsync();
            if (resp.Success && resp.Data != null)
            {
                _historialBase = resp.Data;
                Actividades = _historialBase.ToList();
            }
            else
            {
                MensajeEstado = resp.Message;
                _historialBase = HistorialCambiosMockStore.ObtenerHistorial();
                Actividades = _historialBase.ToList();
            }
            await CargarCatalogosAsync();
        }

        public async Task CargarCatalogosAsync()
        {
            var usuariosResp = await _api.ObtenerUsuariosAsync();
            if (usuariosResp.Success && usuariosResp.Data != null)
                Usuarios = usuariosResp.Data;
            else
                MensajeEstado = usuariosResp.Message;

            var modulosResp = await _api.ObtenerModulosAsync();
            if (modulosResp.Success && modulosResp.Data != null)
                Modulos = modulosResp.Data;
            else if (string.IsNullOrWhiteSpace(MensajeEstado))
                MensajeEstado = modulosResp.Message;
        }

        public void Filtrar(string usuario, string modulo, DateTime? fechaDesde, DateTime? fechaHasta)
        {
            IEnumerable<HistorialCambioItem> consulta = _historialBase;

            if (!string.IsNullOrWhiteSpace(usuario) && usuario != "Todos")
                consulta = consulta.Where(h => h.Usuario.Equals(usuario, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrWhiteSpace(modulo) && modulo != "Todos")
                consulta = consulta.Where(h => h.Modulo.Equals(modulo, StringComparison.OrdinalIgnoreCase));

            if (fechaDesde.HasValue && !fechaHasta.HasValue)
                consulta = consulta.Where(h => h.FechaHora.Date == fechaDesde.Value.Date);

            if (fechaDesde.HasValue && fechaHasta.HasValue)
                consulta = consulta.Where(h =>
                    h.FechaHora.Date >= fechaDesde.Value.Date &&
                    h.FechaHora.Date <= fechaHasta.Value.Date);

            Actividades = consulta.OrderByDescending(h => h.FechaHora).ToList();
        }

        public void LimpiarFiltros()
        {
            Actividades = _historialBase.OrderByDescending(h => h.FechaHora).ToList();
        }
    }
}
