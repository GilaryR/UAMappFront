using SistemaHorario.UI.Models.UI;
using SistemaHorario.UI.Services;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace SistemaHorario.UI.ViewModels.Horarios
{
    /// <summary>
    /// ViewModel principal del módulo Horarios.
    ///
    /// Administra la lista visual de horarios generados,
    /// filtros, paginación y acciones temporales.
    ///
    /// Ahora consume el backend cuando está disponible.
    /// </summary>
    public class HorariosViewModel
    {
        private readonly HorariosApiService _api = new();

        /// <summary>
        /// Lista completa de horarios generados.
        /// </summary>
        public ObservableCollection<HorarioItem> Horarios { get; set; }

        /// <summary>
        /// Lista filtrada y paginada para mostrar en tabla.
        /// </summary>
        public ObservableCollection<HorarioItem> HorariosFiltrados { get; set; }

        /// <summary>
        /// Texto de búsqueda por nombre, grupo, tipo, jornada o estado.
        /// </summary>
        public string Busqueda { get; set; } = string.Empty;

        /// <summary>
        /// Filtro actual por jornada.
        /// </summary>
        public string JornadaSeleccionada { get; set; } = "Todas";

        /// <summary>
        /// Filtro actual por estado.
        /// </summary>
        public string EstadoSeleccionado { get; set; } = "Todos";

        /// <summary>
        /// Página actual de la tabla.
        /// </summary>
        public int PaginaActual { get; private set; } = 1;

        /// <summary>
        /// Cantidad de registros por página.
        /// </summary>
        public int RegistrosPorPagina { get; set; } = 5;

        /// <summary>
        /// Total de páginas calculado según filtros.
        /// </summary>
        public int TotalPaginas { get; private set; } = 1;

        public string MensajeEstado { get; private set; } = string.Empty;

        /// <summary>
        /// Constructor principal.
        /// </summary>
        public HorariosViewModel()
        {
            Horarios = new ObservableCollection<HorarioItem>();
            HorariosFiltrados = new ObservableCollection<HorarioItem>();
        }

        public async Task CargarDatosAsync()
        {
            var resp = await _api.ObtenerHorariosAsync();

            Horarios.Clear();

            if (!resp.Success || resp.Data == null)
            {
                MensajeEstado = resp.Message;
                foreach (HorarioItem horario in HorariosMockStore.ObtenerHorarios())
                {
                    Horarios.Add(horario);
                }
            }
            else
            {
                foreach (HorarioItem horario in resp.Data)
                {
                    Horarios.Add(horario);
                }
            }

            PaginaActual = 1;
            AplicarFiltros();
        }

        /// <summary>
        /// Aplica búsqueda, filtros y paginación.
        /// </summary>
        public void AplicarFiltros()
        {
            var resultado = Horarios.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(Busqueda))
            {
                string busquedaNormalizada = Busqueda.ToLower();

                resultado = resultado.Where(h =>
                    h.Nombre.ToLower().Contains(busquedaNormalizada) ||
                    h.Grupo.ToLower().Contains(busquedaNormalizada) ||
                    h.Tipo.ToLower().Contains(busquedaNormalizada) ||
                    h.Jornada.ToLower().Contains(busquedaNormalizada) ||
                    h.Estado.ToLower().Contains(busquedaNormalizada));
            }

            if (JornadaSeleccionada != "Todas")
            {
                resultado = resultado.Where(h =>
                    h.Jornada == JornadaSeleccionada);
            }

            if (EstadoSeleccionado != "Todos")
            {
                resultado = resultado.Where(h =>
                    h.Estado == EstadoSeleccionado);
            }

            List<HorarioItem> listaFiltrada = resultado.ToList();

            TotalPaginas =
                listaFiltrada.Count == 0
                    ? 1
                    : (int)Math.Ceiling(
                        listaFiltrada.Count / (double)RegistrosPorPagina
                    );

            if (PaginaActual > TotalPaginas)
                PaginaActual = TotalPaginas;

            HorariosFiltrados.Clear();

            foreach (HorarioItem horario in listaFiltrada
                .Skip((PaginaActual - 1) * RegistrosPorPagina)
                .Take(RegistrosPorPagina))
            {
                HorariosFiltrados.Add(horario);
            }
        }

        /// <summary>
        /// Reinicia búsqueda y filtros.
        /// </summary>
        public void LimpiarFiltros()
        {
            Busqueda = string.Empty;
            JornadaSeleccionada = "Todas";
            EstadoSeleccionado = "Todos";
            PaginaActual = 1;

            AplicarFiltros();
        }

        /// <summary>
        /// Avanza a la siguiente página.
        /// </summary>
        public void SiguientePagina()
        {
            if (PaginaActual >= TotalPaginas)
                return;

            PaginaActual++;
            AplicarFiltros();
        }

        /// <summary>
        /// Retrocede a la página anterior.
        /// </summary>
        public void PaginaAnterior()
        {
            if (PaginaActual <= 1)
                return;

            PaginaActual--;
            AplicarFiltros();
        }

        public async Task<bool> EliminarHorarioAsync(HorarioItem horario)
        {
            var resp = await _api.EliminarHorarioAsync(horario.IdHorario);
            if (!resp.Success)
            {
                MensajeEstado = resp.Message;
                return false;
            }

            Horarios.Remove(horario);
            AplicarFiltros();
            return true;
        }
    }
}