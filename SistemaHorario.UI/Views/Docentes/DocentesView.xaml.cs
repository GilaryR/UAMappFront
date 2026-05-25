using SistemaHorario.UI.Controls;
using SistemaHorario.UI.Dialogs.Docentes;
using SistemaHorario.UI.Dialogs.Shared;
using SistemaHorario.UI.Models.UI;
using SistemaHorario.UI.ViewModels.Docentes;
using SistemaHorario.UI.Views.Horarios;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SistemaHorario.UI.Views.Docentes
{
    public partial class DocentesView : UserControl
    {
        private readonly DocentesViewModel _viewModel;

        public DocentesView()
        {
            InitializeComponent();

            _viewModel = new DocentesViewModel();

            ConfigurarTabla();

            Loaded += DocentesView_Loaded;
        }

        private async void DocentesView_Loaded(
            object sender,
            RoutedEventArgs e)
        {
            await RecargarDocentesAsync();
        }

        private void ConfigurarTabla()
        {
            TablaDocentes.ConfigurarColumnas(
            [
                new TableColumnDefinition
                {
                    Header = "Nombre",
                    Binding = "NombreCompleto",
                    Width = 2
                },
                new TableColumnDefinition
                {
                    Header = "Identificación",
                    Binding = "Identificacion",
                    Width = 1
                },
                new TableColumnDefinition
                {
                    Header = "Correo institucional",
                    Binding = "CorreoInstitucional",
                    Width = 2
                },
                new TableColumnDefinition
                {
                    Header = "Materias",
                    Binding = "Materias",
                    Width = 1.5
                },
                new TableColumnDefinition
                {
                    Header = "Estado",
                    Binding = "Estado",
                    Width = 1
                }
            ]);

            TablaDocentes.ConfigurarAcciones(
            [
                new TableActionDefinition
                {
                    Nombre = "editar",
                    Texto = "✏"
                },
                new TableActionDefinition
                {
                    Nombre = "estado",
                    Texto = "↻"
                },
                new TableActionDefinition
                {
                    Nombre = "ver",
                    Texto = "👁"
                },
                new TableActionDefinition
                {
                    Nombre = "horario",
                    Texto = "🕒"
                }
            ]);

            TablaDocentes.AccionEjecutada += TablaDocentes_AccionEjecutada;
        }

        private void ActualizarResumen()
        {
            TxtTotalDocentes.Text =
                _viewModel.Resumen.TotalDocentes.ToString();

            TxtActivos.Text =
                _viewModel.Resumen.Activos.ToString();

            TxtInactivos.Text =
                _viewModel.Resumen.Inactivos.ToString();

            TxtDisponibles.Text =
                _viewModel.Resumen.Disponibles.ToString();
        }

        private void AplicarFiltros()
        {
            _viewModel.Filtrar(
                SearchDocentes.TextoBusqueda,
                ObtenerEstadoSeleccionado());

            TablaDocentes.CargarDatos(_viewModel.Docentes);

            ActualizarResumen();
        }

        private string ObtenerEstadoSeleccionado()
        {
            if (CmbEstadoFiltro.SelectedItem is ComboBoxItem item)
            {
                return item.Content?.ToString() ?? "Todos";
            }

            return "Todos";
        }

        private void SearchDocentes_BusquedaCambiada(
            object sender,
            RoutedEventArgs e)
        {
            AplicarFiltros();
        }

        private void CmbEstadoFiltro_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (!IsLoaded)
            {
                return;
            }

            AplicarFiltros();
        }

        private void BtnAplicarFiltros_Click(
            object sender,
            RoutedEventArgs e)
        {
            AplicarFiltros();
        }

        private void BtnLimpiarFiltros_Click(
            object sender,
            RoutedEventArgs e)
        {
            SearchDocentes.Limpiar();

            CmbEstadoFiltro.SelectedIndex = 0;

            AplicarFiltros();
        }

        private void BtnAgregarDocente_Click(
            object sender,
            RoutedEventArgs e)
        {
            AbrirFormularioDocente();
        }

        private void TablaDocentes_AccionEjecutada(
            object? sender,
            TableActionEventArgs e)
        {
            if (e.Fila is not DocenteItem docente)
            {
                return;
            }

            switch (e.Accion.ToLower())
            {
                case "editar":
                    AbrirFormularioDocente(docente);
                    break;

                case "estado":
                    CambiarEstadoDocente(docente);
                    break;

                case "ver":
                    VerDocente(docente);
                    break;

                case "horario":
                    VerHorarioDocente(docente);
                    break;
            }
        }

        private void VerHorarioDocente(DocenteItem docente)
        {
            ContentControl? contentArea = BuscarContentArea();

            if (contentArea == null)
            {
                return;
            }

            contentArea.Content = new VistaPreviaHorarioView(docente);
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

        private void VerDocente(
            DocenteItem docente)
        {
            AgregarEditarDocenteDialog dialog =
                new(docente, true)
                {
                    Owner = Window.GetWindow(this)
                };

            dialog.ShowDialog();
        }

        private async void AbrirFormularioDocente(
            DocenteItem? docente = null)
        {
            AgregarEditarDocenteDialog dialog =
                docente == null
                    ? new AgregarEditarDocenteDialog()
                    : new AgregarEditarDocenteDialog(docente);

            dialog.Owner = Window.GetWindow(this);

            if (dialog.ShowDialog() != true)
            {
                return;
            }

            await RecargarDocentesAsync();
        }

        private void CambiarEstadoDocente(
            DocenteItem docente)
        {
            if (docente.Estado == "Activo")
            {
                InactivarDocente(docente);
                return;
            }

            ReactivarDocente(docente);
        }

        private async void InactivarDocente(
            DocenteItem docente)
        {
            EliminarConfirmacionDialog dialog =
                new(
                    $"¿Deseas inactivar al docente {docente.NombreCompleto}?\nDebes escribir la palabra eliminar.")
                {
                    Owner = Window.GetWindow(this)
                };

            if (dialog.ShowDialog() != true)
            {
                return;
            }

            bool ok =
                await _viewModel.InactivarDocenteAsync(
                    docente.IdDocente);

            if (!ok)
            {
                MessageBox.Show(
                    "Error al inactivar: " + _viewModel.MensajeEstado,
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return;
            }

            await RecargarDocentesAsync();

            MensajeExitoDialog exito =
                new("Docente inactivado correctamente.")
                {
                    Owner = Window.GetWindow(this)
                };

            exito.ShowDialog();
        }

        private async void ReactivarDocente(
            DocenteItem docente)
        {
            bool ok =
                await _viewModel.ActivarDocenteAsync(
                    docente.IdDocente);

            if (!ok)
            {
                MessageBox.Show(
                    "Error al reactivar: " + _viewModel.MensajeEstado,
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return;
            }

            await RecargarDocentesAsync();

            MensajeExitoDialog exito =
                new("Docente reactivado correctamente.")
                {
                    Owner = Window.GetWindow(this)
                };

            exito.ShowDialog();
        }

        private async Task RecargarDocentesAsync()
        {
            await _viewModel.CargarDatosAsync();

            TablaDocentes.CargarDatos(_viewModel.Docentes);

            ActualizarResumen();
        }
    }
}