using SistemaHorario.UI.Models.UI;
using SistemaHorario.UI.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace SistemaHorario.UI.ViewModels.GruposAcademicos
{
    public class GruposAcademicosViewModel : INotifyPropertyChanged
    {
        private const int TamanoPagina = 10;
        private readonly GruposApiService _api = new();
        private ObservableCollection<GrupoAcademicoItem> _todosLosGrupos = new();
        private ObservableCollection<GrupoAcademicoItem> _gruposFiltrados = new();
        private ObservableCollection<GrupoAcademicoItem> _gruposPaginaActual = new();
        private string _codigoGrupoFiltro = string.Empty;
        private string _estadoFiltro = "Todos";
        private string _jornadaFiltro = "Todas";
        private int _paginaActual = 1;

        public ObservableCollection<GrupoAcademicoItem> GruposPaginaActual
        {
            get => _gruposPaginaActual;
            set { _gruposPaginaActual = value; OnPropertyChanged(); }
        }

        public ObservableCollection<string> MateriasDisponibles { get; private set; } = new();
        public ObservableCollection<PlanAcademicoOption> PlanesDisponibles { get; private set; } = new();
        public string CodigoGrupoFiltro { get => _codigoGrupoFiltro; set { _codigoGrupoFiltro = value; OnPropertyChanged(); } }
        public string EstadoFiltro { get => _estadoFiltro; set { _estadoFiltro = value; OnPropertyChanged(); } }
        public string JornadaFiltro { get => _jornadaFiltro; set { _jornadaFiltro = value; OnPropertyChanged(); } }
        public int PaginaActual { get => _paginaActual; set { _paginaActual = value; OnPropertyChanged(); OnPropertyChanged(nameof(TextoPagina)); } }
        public int TotalPaginas => _gruposFiltrados.Count == 0 ? 1 : (int)Math.Ceiling((double)_gruposFiltrados.Count / TamanoPagina);
        public string TextoPagina => "Pagina " + PaginaActual + " de " + TotalPaginas;
        public string MensajeEstado { get; private set; } = string.Empty;

        public event PropertyChangedEventHandler? PropertyChanged;

        public GruposAcademicosViewModel() { _ = CargarDatosAsync(); }

        public async Task CargarDatosAsync()
        {
            var resp = await _api.ObtenerGruposAsync();
            if (resp.Success && resp.Data != null)
                _todosLosGrupos = new ObservableCollection<GrupoAcademicoItem>(resp.Data);
            else MensajeEstado = resp.Message;

            var respPlanes = await _api.ObtenerPlanesParaGrupoAsync();
            if (respPlanes.Success && respPlanes.Data != null)
            {
                PlanesDisponibles = new ObservableCollection<PlanAcademicoOption>(respPlanes.Data);
                OnPropertyChanged(nameof(PlanesDisponibles));
            }
            else
            {
                MensajeEstado = respPlanes.Message;
            }

            var matApi = new MateriasApiService();
            var respM = await matApi.ObtenerMateriasAsync();
            if (respM.Success && respM.Data != null)
            {
                MateriasDisponibles = new ObservableCollection<string>(respM.Data.Select(m => m.Nombre));
                OnPropertyChanged(nameof(MateriasDisponibles));
            }
            AplicarFiltros();
        }

        public void CargarDatos() => _ = CargarDatosAsync();

        public void AplicarFiltros()
        {
            IEnumerable<GrupoAcademicoItem> resultado = _todosLosGrupos;
            if (!string.IsNullOrWhiteSpace(CodigoGrupoFiltro))
            {
                string t = CodigoGrupoFiltro.Trim();
                resultado = resultado.Where(g => g.Codigo.Contains(t, StringComparison.OrdinalIgnoreCase) || g.NombreGrupo.Contains(t, StringComparison.OrdinalIgnoreCase));
            }
            if (!string.IsNullOrWhiteSpace(EstadoFiltro) && EstadoFiltro != "Todos")
                resultado = resultado.Where(g => g.Estado.Equals(EstadoFiltro, StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrWhiteSpace(JornadaFiltro) && JornadaFiltro != "Todas")
                resultado = resultado.Where(g => g.Jornada.Equals(JornadaFiltro, StringComparison.OrdinalIgnoreCase));
            _gruposFiltrados = new ObservableCollection<GrupoAcademicoItem>(resultado);
            PaginaActual = 1;
            CargarPagina();
        }

        public void LimpiarFiltros() { CodigoGrupoFiltro = string.Empty; EstadoFiltro = "Todos"; JornadaFiltro = "Todas"; AplicarFiltros(); }
        public void SiguientePagina() { if (PaginaActual < TotalPaginas) { PaginaActual++; CargarPagina(); } }
        public void PaginaAnterior() { if (PaginaActual > 1) { PaginaActual--; CargarPagina(); } }

        public async Task<bool> AgregarGrupoAsync(GrupoAcademicoItem g)
        {
            var r = await _api.CrearGrupoAsync(g);
            if (r.Success) { await CargarDatosAsync(); return true; }
            MensajeEstado = r.Message; return false;
        }

        public async Task<bool> ActualizarGrupoAsync(GrupoAcademicoItem g)
        {
            var r = await _api.ActualizarGrupoAsync(g);
            if (r.Success) { await CargarDatosAsync(); return true; }
            MensajeEstado = r.Message; return false;
        }

        public async Task<bool> EliminarGrupoAsync(int id)
        {
            var r = await _api.EliminarGrupoAsync(id);
            if (r.Success) { await CargarDatosAsync(); return true; }
            MensajeEstado = r.Message; return false;
        }

        public void AgregarGrupo(GrupoAcademicoItem g) => _ = AgregarGrupoAsync(g);
        public void ActualizarGrupo(GrupoAcademicoItem g) => _ = ActualizarGrupoAsync(g);
        public void EliminarGrupo(int id) => _ = EliminarGrupoAsync(id);

        private void CargarPagina()
        {
            var pagina = _gruposFiltrados.Skip((PaginaActual - 1) * TamanoPagina).Take(TamanoPagina).ToList();
            GruposPaginaActual = new ObservableCollection<GrupoAcademicoItem>(pagina);
            OnPropertyChanged(nameof(TotalPaginas));
            OnPropertyChanged(nameof(TextoPagina));
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
