using SistemaHorario.UI.Controls;
using SistemaHorario.UI.Dialogs.Shared;
using SistemaHorario.UI.Models.UI;
using SistemaHorario.UI.ViewModels.Coordinadores;
using System.Windows;
using System.Windows.Controls;

namespace SistemaHorario.UI.Views.Coordinadores
{
    /// <summary>
    /// Vista principal del módulo Coordinadores.
    /// </summary>
    public partial class CoordinadoresView : UserControl
    {
        private readonly CoordinadoresViewModel _viewModel;

        public CoordinadoresView()
        {
            InitializeComponent();

            _viewModel = new CoordinadoresViewModel();

            ConfigurarTabla();

            Loaded += CoordinadoresView_Loaded;
        }

        private async void CoordinadoresView_Loaded(
            object sender,
            RoutedEventArgs e)
        {
            await _viewModel.CargarDatosAsync();

            if (!string.IsNullOrWhiteSpace(_viewModel.MensajeEstado))
            {
                MessageBox.Show(
                    _viewModel.MensajeEstado,
                    "Error al cargar coordinadores",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }

            CargarDatos();
        }

        private void ConfigurarTabla()
        {
            TablaCoordinadores.ConfigurarColumnas(
            [
                new TableColumnDefinition
                {
                    Header = "Nombre",
                    Binding = "NombreCompleto",
                    Width = 2
                },
                new TableColumnDefinition
                {
                    Header = "Cédula",
                    Binding = "Cedula",
                    Width = 1.1
                },
                new TableColumnDefinition
                {
                    Header = "Correo institucional",
                    Binding = "CorreoInstitucional",
                    Width = 2
                },
                new TableColumnDefinition
                {
                    Header = "Celular",
                    Binding = "Celular",
                    Width = 1.1
                },
                new TableColumnDefinition
                {
                    Header = "Estado",
                    Binding = "Estado",
                    Width = 1
                }
            ]);

            TablaCoordinadores.ConfigurarAcciones(
            [
                new TableActionDefinition
                {
                    Nombre = "editar",
                    Texto = "✏"
                },
                new TableActionDefinition
                {
                    Nombre = "estado",
                    Texto = "⏻"
                }
            ]);

            TablaCoordinadores.AccionEjecutada +=
                TablaCoordinadores_AccionEjecutada;
        }

        private void CargarDatos()
        {
            TablaCoordinadores.CargarDatos(_viewModel.Coordinadores);
        }

        private void SearchCoordinadores_BusquedaCambiada(
            object sender,
            RoutedEventArgs e)
        {
            AplicarFiltros();
        }

        private void BtnFiltrar_Click(object sender, RoutedEventArgs e)
        {
            AplicarFiltros();
        }

        private void BtnLimpiar_Click(object sender, RoutedEventArgs e)
        {
            SearchCoordinadores.Limpiar();
            CmbBuscarPor.SelectedIndex = 0;
            CmbEstado.SelectedIndex = 0;
            AplicarFiltros();
        }

        private void AplicarFiltros()
        {
            _viewModel.Filtrar(
                SearchCoordinadores.TextoBusqueda,
                ObtenerTextoCombo(CmbBuscarPor),
                ObtenerTextoCombo(CmbEstado));

            CargarDatos();
        }

        private static string ObtenerTextoCombo(ComboBox combo)
        {
            if (combo.SelectedItem is ComboBoxItem item)
            {
                return item.Content?.ToString() ?? string.Empty;
            }

            return string.Empty;
        }

        private void TablaCoordinadores_AccionEjecutada(
            object? sender,
            TableActionEventArgs e)
        {
            if (e.Fila is not CoordinadorItem coordinador)
            {
                return;
            }

            switch (e.Accion.ToLower())
            {
                case "editar":
                    EditarCoordinador(coordinador);
                    break;

                case "estado":
                    CambiarEstadoCoordinador(coordinador);
                    break;
            }
        }

        private void BtnAgregar_Click(object sender, RoutedEventArgs e)
        {
            NavegarAFormulario();
        }

        private void EditarCoordinador(CoordinadorItem coordinador)
        {
            NavegarAFormulario(coordinador);
        }

        private async void CambiarEstadoCoordinador(CoordinadorItem coordinador)
        {
            string accion = coordinador.EstaActivo ? "inactivar" : "activar";
            string estadoFinal = coordinador.EstaActivo ? "Inactivo" : "Activo";

            MessageBoxResult confirmacion = MessageBox.Show(
                $"¿Deseas {accion} al coordinador {coordinador.NombreCompleto}?",
                "Confirmar cambio de estado",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (confirmacion != MessageBoxResult.Yes)
            {
                return;
            }

            bool actualizado =
                await _viewModel.CambiarEstadoCoordinadorAsync(coordinador);

            if (!actualizado)
            {
                MessageBox.Show(
                    _viewModel.MensajeEstado,
                    "Error al cambiar estado",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return;
            }

            CargarDatos();

            MensajeExitoDialog exito = new(
                $"Coordinador marcado como {estadoFinal} correctamente.")
            {
                Owner = Window.GetWindow(this)
            };

            exito.ShowDialog();
        }

        private void NavegarAFormulario(CoordinadorItem? coordinador = null)
        {
            ContentControl? contentArea = BuscarContentArea();

            if (contentArea == null)
            {
                return;
            }

            contentArea.Content = new AgregarEditarCoordinadorView(coordinador);
        }

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

                actual = System.Windows.Media.VisualTreeHelper.GetParent(actual);
            }

            return null;
        }
    }
}
