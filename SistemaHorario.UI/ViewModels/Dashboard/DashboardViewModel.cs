using SistemaHorario.UI.Services;
using SistemaHorario.UI.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;

namespace SistemaHorario.UI.ViewModels.Dashboard
{
    /// <summary>
    /// ViewModel del Dashboard. Obtiene y prepara los datos reales del resumen inicial.
    /// </summary>
    public class DashboardViewModel : ViewModelBase
    {
        private readonly DashboardApiService _api = new();
        private readonly HorariosApiService _horariosApi = new();

        private string _totalMaterias = "0";
        private string _totalPlanesAcademicos = "0";
        private string _totalUsuarios = "0";
        private string _totalGrupos = "0";
        private string _mensajeEstado = string.Empty;

        public string TotalMaterias
        {
            get => _totalMaterias;
            private set => SetProperty(ref _totalMaterias, value);
        }

        public string TotalPlanesAcademicos
        {
            get => _totalPlanesAcademicos;
            private set => SetProperty(ref _totalPlanesAcademicos, value);
        }

        public string TotalUsuarios
        {
            get => _totalUsuarios;
            private set => SetProperty(ref _totalUsuarios, value);
        }

        public string TotalGrupos
        {
            get => _totalGrupos;
            private set => SetProperty(ref _totalGrupos, value);
        }

        public string MensajeEstado
        {
            get => _mensajeEstado;
            private set => SetProperty(ref _mensajeEstado, value);
        }

        public ObservableCollection<HorarioGeneradoItem> UltimosHorarios { get; } = new();

        /// <summary>
        /// Consulta el resumen general y los últimos horarios desde la API.
        /// </summary>
        public async Task CargarResumenAsync()
        {
            await CargarTotalesAsync();
            await CargarUltimosHorariosAsync();
        }

        private async Task CargarTotalesAsync()
        {
            var resp = await _api.ObtenerResumenAsync();

            if (!resp.Success || resp.Data == null)
            {
                LimpiarTotales();
                MensajeEstado = string.IsNullOrWhiteSpace(resp.Message)
                    ? "No se pudo cargar el resumen del dashboard."
                    : resp.Message;

                return;
            }

            TotalMaterias = resp.Data.TotalMaterias.ToString();
            TotalPlanesAcademicos = resp.Data.TotalPlanesAcademicos.ToString();
            TotalUsuarios = resp.Data.TotalUsuarios.ToString();
            TotalGrupos = resp.Data.TotalGrupos.ToString();
            MensajeEstado = string.Empty;
        }

        private async Task CargarUltimosHorariosAsync()
        {
            UltimosHorarios.Clear();

            var resp = await _horariosApi.ObtenerHorariosAsync();

            if (!resp.Success || resp.Data == null || resp.Data.Count == 0)
            {
                return;
            }

            foreach (var h in resp.Data.Take(5))
            {
                UltimosHorarios.Add(new HorarioGeneradoItem
                {
                    Nombre = h.Nombre,
                    Fecha = h.FechaGeneracion,
                    Jornada = h.Jornada,
                    Grupos = h.Grupo,
                    Semestre = "-",
                    Estado = h.Estado,
                    JornadaFondo = ObtenerFondoJornada(h.Jornada),
                    JornadaColorTexto = ObtenerTextoJornada(h.Jornada)
                });
            }
        }

        private void LimpiarTotales()
        {
            TotalMaterias = "0";
            TotalPlanesAcademicos = "0";
            TotalUsuarios = "0";
            TotalGrupos = "0";
        }

        private static SolidColorBrush ObtenerFondoJornada(string jornada)
        {
            return jornada.ToLower() switch
            {
                "nocturna" => new SolidColorBrush(Color.FromRgb(221, 206, 255)),
                "diurna" => new SolidColorBrush(Color.FromRgb(191, 225, 247)),
                _ => new SolidColorBrush(Color.FromRgb(220, 220, 220))
            };
        }

        private static SolidColorBrush ObtenerTextoJornada(string jornada)
        {
            return jornada.ToLower() switch
            {
                "nocturna" => new SolidColorBrush(Color.FromRgb(102, 93, 214)),
                "diurna" => new SolidColorBrush(Color.FromRgb(0, 106, 166)),
                _ => new SolidColorBrush(Color.FromRgb(80, 80, 80))
            };
        }
    }

    /// <summary>
    /// Modelo visual para representar horarios recientes dentro del Dashboard.
    /// </summary>
    public class HorarioGeneradoItem
    {
        public string Nombre { get; set; } = string.Empty;
        public string Fecha { get; set; } = string.Empty;
        public string Jornada { get; set; } = string.Empty;
        public string Grupos { get; set; } = string.Empty;
        public string Semestre { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public Brush JornadaFondo { get; set; } = Brushes.Transparent;
        public Brush JornadaColorTexto { get; set; } = Brushes.Black;
    }
}