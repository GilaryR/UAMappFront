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

        public CoordinadoresViewModel()
        {
        }

        public async Task CargarDatosAsync()
        {
            var resp = await _api.ObtenerCoordinadoresAsync();
            if (!resp.Success || resp.Data == null)
            {
                MensajeEstado = resp.Message;
                return;
            }
            _coordinadoresBase = resp.Data;
            Coordinadores = _coordinadoresBase.ToList();
            CalcularResumen();
        }

        public void Filtrar(string textoBusqueda, string filtroBusqueda, string estado)
        {
            IEnumerable<CoordinadorItem> query = _coordinadoresBase;
            if (!string.IsNullOrWhiteSpace(textoBusqueda))
            {
                textoBusqueda = textoBusqueda.ToLower();
                switch (filtroBusqueda.ToLower())
                {
                    case "nombre":
                        query = query.Where(x => x.NombreCompleto.ToLower().Contains(textoBusqueda));
                        break;
                    case "cedula":
                        query = query.Where(x => x.Cedula.ToLower().Contains(textoBusqueda));
                        break;
                    case "correo":
                        query = query.Where(x => x.CorreoInstitucional.ToLower().Contains(textoBusqueda));
                        break;
                }
            }
            if (estado != "Todos")
                query = query.Where(x => x.Estado == estado);
            Coordinadores = query.ToList();
            CalcularResumen();
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
