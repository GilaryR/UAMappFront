using SistemaHorario.UI.Dialogs.Shared;
using SistemaHorario.UI.Models.UI;
using SistemaHorario.UI.ViewModels.Coordinadores;
using System.Windows;
using System.Windows.Controls;

namespace SistemaHorario.UI.Views.Coordinadores
{
    public partial class AgregarEditarCoordinadorView : UserControl
    {
        private readonly AgregarEditarCoordinadorViewModel _viewModel;

        public AgregarEditarCoordinadorView(CoordinadorItem? coordinador = null)
        {
            InitializeComponent();

            _viewModel = coordinador == null
                ? new AgregarEditarCoordinadorViewModel()
                : new AgregarEditarCoordinadorViewModel(coordinador);

            CargarDatos();
        }

        private void CargarDatos()
        {
            TxtTitulo.Text = _viewModel.EsEdicion
                ? "Editar coordinador"
                : "Agregar coordinador";

            TxtNombre.Text = _viewModel.Coordinador.NombreCompleto;
            TxtCedula.Text = _viewModel.Coordinador.Cedula;
            TxtCorreo.Text = _viewModel.Coordinador.CorreoInstitucional;
            TxtCelular.Text = _viewModel.Coordinador.Celular;
            TxtRol.Text = "Coordinador";

            SeleccionarEstado(_viewModel.Coordinador.Estado);

            if (_viewModel.EsEdicion)
            {
                PnlContrasena.Visibility = Visibility.Collapsed;
                PnlAyudaContrasena.Visibility = Visibility.Collapsed;
            }
            else
            {
                PwdContrasena.Password = _viewModel.Coordinador.ContrasenaInicial;
            }
        }

        private void SeleccionarEstado(string estado)
        {
            foreach (ComboBoxItem item in CmbEstado.Items)
            {
                if (item.Content?.ToString() == estado)
                {
                    CmbEstado.SelectedItem = item;
                    return;
                }
            }

            CmbEstado.SelectedIndex = 0;
        }

        private async void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            if (!FormularioEsValido())
            {
                return;
            }

            _viewModel.Coordinador.NombreCompleto = TxtNombre.Text.Trim();
            _viewModel.Coordinador.Cedula = TxtCedula.Text.Trim();
            _viewModel.Coordinador.CorreoInstitucional = TxtCorreo.Text.Trim();
            _viewModel.Coordinador.Celular = TxtCelular.Text.Trim();
            _viewModel.Coordinador.Rol = "Coordinador";

            if (!_viewModel.EsEdicion)
            {
                _viewModel.Coordinador.ContrasenaInicial =
                    PwdContrasena.Password.Trim();
            }

            if (CmbEstado.SelectedItem is ComboBoxItem item)
            {
                _viewModel.Coordinador.Estado =
                    item.Content?.ToString() ?? "Activo";
            }

            bool guardado = await _viewModel.GuardarAsync();

            if (!guardado)
            {
                MessageBox.Show(
                    _viewModel.MensajeEstado,
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                return;
            }

            MensajeExitoDialog dialog = new(
                _viewModel.EsEdicion
                    ? "Coordinador actualizado correctamente."
                    : "Coordinador creado correctamente.")
            {
                Owner = Window.GetWindow(this)
            };

            dialog.ShowDialog();
            VolverAPrincipal();
        }

        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            VolverAPrincipal();
        }

        private bool FormularioEsValido()
        {
            if (string.IsNullOrWhiteSpace(TxtNombre.Text))
            {
                MostrarValidacion("El nombre completo es obligatorio.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(TxtCedula.Text))
            {
                MostrarValidacion("La cédula es obligatoria.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(TxtCorreo.Text))
            {
                MostrarValidacion("El correo institucional es obligatorio.");
                return false;
            }

            if (!TxtCorreo.Text.Contains("@"))
            {
                MostrarValidacion("El correo institucional no tiene un formato válido.");
                return false;
            }

            if (!_viewModel.EsEdicion && string.IsNullOrWhiteSpace(PwdContrasena.Password))
            {
                MostrarValidacion("La contraseña inicial es obligatoria.");
                return false;
            }

            if (!_viewModel.EsEdicion && PwdContrasena.Password.Length < 6)
            {
                MostrarValidacion("La contraseña inicial debe tener al menos 6 caracteres.");
                return false;
            }

            return true;
        }

        private static void MostrarValidacion(string mensaje)
        {
            MessageBox.Show(
                mensaje,
                "Validación",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }

        private void VolverAPrincipal()
        {
            ContentControl? contentArea = BuscarContentArea();

            if (contentArea == null)
            {
                return;
            }

            contentArea.Content = new CoordinadoresView();
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
