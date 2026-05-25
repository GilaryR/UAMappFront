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
    /// Muestra un registro por grupo académico.
    /// </summary>
    public partial class HorariosView : UserControl
    {
        private readonly HorariosViewModel _viewModel;

        public HorariosView()
        {
            InitializeComponent();

            _viewModel = new HorariosViewModel();

            ConfigurarTabla();

            Loaded += HorariosView_Loaded;
        }

        private async void HorariosView_Loaded(object sender, RoutedEventArgs e)
        {
            await _viewModel.CargarDatosAsync();
            CargarTabla();
        }

        private void ConfigurarTabla()
        {
            TablaHorarios.ConfigurarColumnas(
                new List<TableColumnDefinition>
                {
                    new TableColumnDefinition
                    {
                        Header = "Horario",
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
                        Header = "Bloques",
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

        private void CargarTabla()
        {
            TablaHorarios.CargarDatos(_viewModel.HorariosFiltrados.ToList());
        }

        private void TablaHorarios_AccionEjecutada(object? sender, TableActionEventArgs e)
        {
            if (e.Fila is not HorarioItem horario)
            {
                return;
            }

            switch (e.Accion.ToLower())
            {
                case "ver":
                    VerHorario(horario);
                    break;

                case "editar":
                    EditarHorario(horario);
                    break;

                case "eliminar":
                    EliminarHorarioGrupo(horario);
                    break;
            }
        }

        private void BtnGenerarHorario_Click(object sender, RoutedEventArgs e)
        {
            NavegarA(new GenerarHorarioView());
        }

        private void SearchBoxHorarios_BusquedaCambiada(object sender, RoutedEventArgs e)
        {
            _viewModel.Busqueda = SearchBoxHorarios.TextoBusqueda;
            _viewModel.AplicarFiltros();
            CargarTabla();
        }

        private void CmbJornada_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_viewModel == null || CmbJornada.SelectedItem is not ComboBoxItem item)
            {
                return;
            }

            _viewModel.JornadaSeleccionada = item.Content?.ToString() ?? "Todas";
            _viewModel.AplicarFiltros();
            CargarTabla();
        }

        private void CmbEstado_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_viewModel == null || CmbEstado.SelectedItem is not ComboBoxItem item)
            {
                return;
            }

            _viewModel.EstadoSeleccionado = item.Content?.ToString() ?? "Todos";
            _viewModel.AplicarFiltros();
            CargarTabla();
        }

        private void BtnLimpiarFiltros_Click(object sender, RoutedEventArgs e)
        {
            SearchBoxHorarios.Limpiar();
            CmbJornada.SelectedIndex = 0;
            CmbEstado.SelectedIndex = 0;
            _viewModel.LimpiarFiltros();
            CargarTabla();
        }

        private void VerHorario(HorarioItem horario)
        {
            NavegarA(new VistaPreviaHorarioView(horario, false, false));
        }

        private void EditarHorario(HorarioItem horario)
        {
            NavegarA(new VistaPreviaHorarioView(horario, true, true));
        }

        private async void EliminarHorarioGrupo(HorarioItem horario)
        {
            EliminarConfirmacionDialog dialog = new()
            {
                Owner = Window.GetWindow(this)
            };

            bool? resultado = dialog.ShowDialog();

            if (resultado != true)
            {
                return;
            }

            bool ok = await _viewModel.EliminarHorarioGrupoAsync(horario);
            if (!ok)
            {
                MessageBox.Show(
                    $"No se pudo eliminar el horario del grupo:\n{_viewModel.MensajeEstado}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return;
            }

            CargarTabla();
        }

        private void NavegarA(UserControl vista)
        {
            ContentControl? contentArea = BuscarContentArea();

            if (contentArea == null)
            {
                return;
            }

            contentArea.Content = vista;
        }

        private ContentControl? BuscarContentArea()
        {
            DependencyObject? actual = this;

            while (actual != null)
            {
                if (actual is ContentControl content && content.Name == "ContentArea")
                {
                    return content;
                }

                actual = System.Windows.Media.VisualTreeHelper.GetParent(actual);
            }

            return null;
        }
    }
}
