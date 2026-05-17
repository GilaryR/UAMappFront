using SistemaHorario.UI.Controls;
using SistemaHorario.UI.Models.UI;
using SistemaHorario.UI.Dialogs.Shared;
using SistemaHorario.UI.ViewModels.Horarios;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SistemaHorario.UI.Views.Horarios
{
    /// <summary>
    /// Vista principal del módulo Horarios.
    ///
    /// Permite:
    /// - Consultar horarios generados.
    /// - Filtrar resultados.
    /// - Buscar horarios.
    /// - Visualizar vista previa.
    /// - Editar horarios.
    /// - Eliminar horarios.
    /// - Navegar hacia generación automática.
    ///
    /// Actualmente trabaja con datos mock.
    ///
    /// Más adelante consumirá:
    ///
    /// GET /api/horarios
    /// GET /api/horarios/{id}
    /// GET /api/horarios/{id}/vista-previa
    /// POST /api/horarios/generar
    /// POST /api/horarios/{id}/aprobar
    /// POST /api/horarios/{id}/rechazar
    /// </summary>
    public partial class HorariosView : UserControl
    {
        /// <summary>
        /// ViewModel asociado a la vista.
        /// </summary>
        private readonly HorariosViewModel _viewModel;

        /// <summary>
        /// Constructor principal.
        /// </summary>
        public HorariosView()
        {
            InitializeComponent();

            _viewModel = new HorariosViewModel();

            ConfigurarTabla();

            CargarTabla();
        }

        /// <summary>
        /// Configura columnas dinámicas y acciones
        /// reutilizando TablaPaginada.
        /// </summary>
        private void ConfigurarTabla()
        {
            TablaHorarios.ConfigurarColumnas(
                new List<TableColumnDefinition>
                {
            new TableColumnDefinition
            {
                Header = "Nombre",
                Binding = "Nombre",
                Width = 2
            },

            new TableColumnDefinition
            {
                Header = "Grupo",
                Binding = "Grupo",
                Width = 1
            },

            new TableColumnDefinition
            {
                Header = "Tipo",
                Binding = "Tipo",
                Width = 1
            },

            new TableColumnDefinition
            {
                Header = "Jornada",
                Binding = "Jornada",
                Width = 1
            },

            new TableColumnDefinition
            {
                Header = "Fecha",
                Binding = "FechaGeneracion",
                Width = 1
            },

            new TableColumnDefinition
            {
                Header = "Estado",
                Binding = "Estado",
                Width = 1
            }
                });

            TablaHorarios.ConfigurarAcciones(
                new List<TableActionDefinition>
                {
            new TableActionDefinition
            {
                Nombre = "ver",
                Texto = "👁"
            },

            new TableActionDefinition
            {
                Nombre = "editar",
                Texto = "✏"
            },

            new TableActionDefinition
            {
                Nombre = "eliminar",
                Texto = "🗑"
            }
                });

            TablaHorarios.AccionEjecutada += TablaHorarios_AccionEjecutada;
        }

        /// <summary>
        /// Carga información en la tabla reutilizable.
        /// </summary>
        private void CargarTabla()
        {
            TablaHorarios.CargarDatos(
                _viewModel.HorariosFiltrados.ToList());
        }

        /// <summary>
        /// Maneja acciones dinámicas de la tabla.
        /// </summary>
        private void TablaHorarios_AccionEjecutada(
            object? sender,
            TableActionEventArgs e)
        {
            if (e.Fila is not HorarioItem horario)
                return;

            switch (e.Accion.ToLower())
            {
                case "ver":
                    VerHorario(horario);
                    break;

                case "editar":
                    EditarHorario(horario);
                    break;

                case "eliminar":
                    EliminarHorario(horario);
                    break;
            }
        }

        /// <summary>
        /// Navega hacia generación automática.
        /// </summary>
        private void BtnGenerarHorario_Click(
            object sender,
            RoutedEventArgs e)
        {
            ContentControl? contentArea =
                BuscarContentArea();

            if (contentArea == null)
                return;

            contentArea.Content =
                new GenerarHorarioView();
        }

        /// <summary>
        /// Filtra horarios por búsqueda textual.
        /// </summary>
        private void SearchBoxHorarios_BusquedaCambiada(
            object sender,
            RoutedEventArgs e)
        {
            _viewModel.Busqueda =
                SearchBoxHorarios.TextoBusqueda;

            _viewModel.AplicarFiltros();

            CargarTabla();
        }

        /// <summary>
        /// Filtra horarios por jornada.
        /// </summary>
        private void CmbJornada_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (_viewModel == null)
                return;

            if (CmbJornada.SelectedItem is not ComboBoxItem item)
                return;

            _viewModel.JornadaSeleccionada =
                item.Content?.ToString() ?? "Todas";

            _viewModel.AplicarFiltros();

            CargarTabla();
        }

        /// <summary>
        /// Filtra horarios por estado.
        /// </summary>
        private void CmbEstado_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (_viewModel == null)
                return;

            if (CmbEstado.SelectedItem is not ComboBoxItem item)
                return;

            _viewModel.EstadoSeleccionado =
                item.Content?.ToString() ?? "Todos";

            _viewModel.AplicarFiltros();

            CargarTabla();
        }

        /// <summary>
        /// Limpia búsqueda y filtros.
        /// </summary>
        private void BtnLimpiarFiltros_Click(
            object sender,
            RoutedEventArgs e)
        {
            SearchBoxHorarios.Limpiar();

            CmbJornada.SelectedIndex = 0;

            CmbEstado.SelectedIndex = 0;

            _viewModel.LimpiarFiltros();

            CargarTabla();
        }

        /// <summary>
        /// Acción visual temporal para vista previa.
        ///
        /// TODO:
        /// Navegar hacia VistaPreviaHorarioView.
        /// </summary>
        private void VerHorario(HorarioItem horario)
        {
            ContentControl? contentArea = BuscarContentArea();

            if (contentArea == null)
                return;

            contentArea.Content =
                new VistaPreviaHorarioView(horario, false, false);
        }

        /// <summary>
        /// Acción visual temporal para edición.
        ///
        /// TODO:
        /// Navegar hacia editor drag & drop.
        /// </summary>
        private void EditarHorario(HorarioItem horario)
        {
            ContentControl? contentArea = BuscarContentArea();

            if (contentArea == null)
                return;

            contentArea.Content =
                new VistaPreviaHorarioView(horario, true, false);
        }

        /// <summary>
        /// Solicita confirmación visual y elimina temporalmente
        /// el horario seleccionado.
        ///
        /// TODO:
        /// Cuando backend agregue DELETE /api/horarios/{id},
        /// reemplazar eliminación local por consumo real del endpoint.
        /// </summary>
        private void EliminarHorario(HorarioItem horario)
        {
            EliminarConfirmacionDialog dialog = new()
            {
                Owner = Window.GetWindow(this)
            };

            bool? resultado = dialog.ShowDialog();

            if (resultado != true)
                return;

            _viewModel.EliminarHorario(horario);

            CargarTabla();
        }

        /// <summary>
        /// Busca el ContentArea principal.
        /// </summary>
        private ContentControl? BuscarContentArea()
        {
            DependencyObject? actual = this;

            while (actual != null)
            {
                if (actual is ContentControl content &&
                    content.Name == "ContentArea")
                {
                    return content;
                }

                actual =
                    System.Windows.Media.VisualTreeHelper.GetParent(actual);
            }

            return null;
        }
    }
}