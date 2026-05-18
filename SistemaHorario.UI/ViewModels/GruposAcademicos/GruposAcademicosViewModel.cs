using SistemaHorario.UI.Models.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace SistemaHorario.UI.ViewModels.GruposAcademicos
{
    /// <summary>
    /// Controla la información que usa la vista de grupos académicos.
    /// 
    /// Aquí se cargan los grupos, se aplican filtros y se maneja la paginación.
    /// Cuando se conecte el backend, este archivo puede seguir siendo útil:
    /// solo se cambia el origen de los datos.
    /// </summary>
    public class GruposAcademicosViewModel : INotifyPropertyChanged
    {
        private const int TamanoPagina = 10;

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
            set
            {
                _gruposPaginaActual = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> MateriasDisponibles { get; private set; } = new();

        public string CodigoGrupoFiltro
        {
            get => _codigoGrupoFiltro;
            set
            {
                _codigoGrupoFiltro = value;
                OnPropertyChanged();
            }
        }

        public string EstadoFiltro
        {
            get => _estadoFiltro;
            set
            {
                _estadoFiltro = value;
                OnPropertyChanged();
            }
        }

        public string JornadaFiltro
        {
            get => _jornadaFiltro;
            set
            {
                _jornadaFiltro = value;
                OnPropertyChanged();
            }
        }

        public int PaginaActual
        {
            get => _paginaActual;
            set
            {
                _paginaActual = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TextoPagina));
            }
        }

        public int TotalPaginas
        {
            get
            {
                if (_gruposFiltrados.Count == 0)
                    return 1;

                return (int)Math.Ceiling((double)_gruposFiltrados.Count / TamanoPagina);
            }
        }

        public string TextoPagina => $"Página {PaginaActual} de {TotalPaginas}";

        public event PropertyChangedEventHandler? PropertyChanged;

        public GruposAcademicosViewModel()
        {
            CargarDatos();
        }

        public void CargarDatos()
        {
            _todosLosGrupos = GruposAcademicosMockStore.ObtenerGrupos();
            MateriasDisponibles = GruposAcademicosMockStore.ObtenerMateriasDisponibles();

            AplicarFiltros();
        }

        public void AplicarFiltros()
        {
            IEnumerable<GrupoAcademicoItem> resultado = _todosLosGrupos;

            if (!string.IsNullOrWhiteSpace(CodigoGrupoFiltro))
            {
                string textoBusqueda = CodigoGrupoFiltro.Trim();

                resultado = resultado.Where(grupo =>
                    grupo.Codigo.Contains(textoBusqueda, StringComparison.OrdinalIgnoreCase)
                    || grupo.NombreGrupo.Contains(textoBusqueda, StringComparison.OrdinalIgnoreCase)
                );
            }

            if (!string.IsNullOrWhiteSpace(EstadoFiltro) &&
                EstadoFiltro != "Todos")
            {
                resultado = resultado.Where(grupo =>
                    grupo.Estado.Equals(EstadoFiltro, StringComparison.OrdinalIgnoreCase)
                );
            }

            if (!string.IsNullOrWhiteSpace(JornadaFiltro) &&
                JornadaFiltro != "Todas")
            {
                resultado = resultado.Where(grupo =>
                    grupo.Jornada.Equals(JornadaFiltro, StringComparison.OrdinalIgnoreCase)
                );
            }

            _gruposFiltrados = new ObservableCollection<GrupoAcademicoItem>(resultado);

            PaginaActual = 1;
            CargarPagina();
        }

        public void LimpiarFiltros()
        {
            CodigoGrupoFiltro = string.Empty;
            EstadoFiltro = "Todos";
            JornadaFiltro = "Todas";

            AplicarFiltros();
        }

        public void SiguientePagina()
        {
            if (PaginaActual >= TotalPaginas)
                return;

            PaginaActual++;
            CargarPagina();
        }

        public void PaginaAnterior()
        {
            if (PaginaActual <= 1)
                return;

            PaginaActual--;
            CargarPagina();
        }

        public void AgregarGrupo(GrupoAcademicoItem grupo)
        {
            GruposAcademicosMockStore.AgregarGrupo(grupo);
            AplicarFiltros();
        }

        public void ActualizarGrupo(GrupoAcademicoItem grupo)
        {
            GruposAcademicosMockStore.ActualizarGrupo(grupo);
            AplicarFiltros();
        }

        public void EliminarGrupo(int idGrupoAcademico)
        {
            GruposAcademicosMockStore.EliminarGrupo(idGrupoAcademico);
            AplicarFiltros();
        }

        private void CargarPagina()
        {
            List<GrupoAcademicoItem> pagina = _gruposFiltrados
                .Skip((PaginaActual - 1) * TamanoPagina)
                .Take(TamanoPagina)
                .ToList();

            GruposPaginaActual = new ObservableCollection<GrupoAcademicoItem>(pagina);

            OnPropertyChanged(nameof(TotalPaginas));
            OnPropertyChanged(nameof(TextoPagina));
        }

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(propertyName)
            );
        }
    }
}