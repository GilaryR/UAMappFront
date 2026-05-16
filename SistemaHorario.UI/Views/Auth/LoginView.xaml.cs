using SistemaHorario.UI.ViewModels.Auth;
using SistemaHorario.UI.Views.Shell;
using SistemaHorarios.Application.Common;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SistemaHorario.UI.Views.Auth
{
    /// <summary>
    /// Vista de inicio de sesión del sistema.
    ///
    /// Esta pantalla permite:
    /// - Capturar el correo institucional.
    /// - Capturar la contraseña.
    /// - Validar campos obligatorios.
    /// - Probar la conexión con la API mediante un GET temporal.
    /// - Navegación hacia MainShellView.
    ///
    /// NOTA:
    /// En esta etapa todavía no se consume el endpoint real de login.
    /// El formulario queda preparado para que posteriormente se conecte
    /// con /api/auth/login.
    /// </summary>
    public partial class LoginView : UserControl
    {
        /// <summary>
        /// ViewModel asociado al login.
        /// </summary>
        private readonly LoginViewModel _viewModel;

        /// <summary>
        /// Constructor de la vista LoginView.
        ///
        /// Inicializa los componentes visuales, instancia el servicio
        /// de prueba de conexión y registra eventos para actualizar
        /// los placeholders de los campos.
        /// </summary>
        public LoginView()
        {
            InitializeComponent();

            _viewModel = new LoginViewModel();
            DataContext = _viewModel;

            TxtCorreoInstitucional.TextChanged += TxtCorreoInstitucional_TextChanged;
            TxtContrasena.PasswordChanged += TxtContrasena_PasswordChanged;

            ActualizarPlaceholders();
        }

        /// <summary>
        /// Evento ejecutado al presionar Iniciar sesión.
        ///
        /// Actualmente el sistema NO consume todavía
        /// el endpoint real POST /api/auth/login.
        ///
        /// Por esta razón:
        /// - solo se validan campos básicos
        /// - se permite navegar temporalmente al sistema
        ///
        /// TODO:
        /// Reemplazar esta lógica por autenticación real
        /// cuando backend implemente JWT/login completo.
        /// </summary>
        private void BtnIniciarSesion_Click(object sender, RoutedEventArgs e)
        {
            LimpiarMensajes();

            SincronizarViewModel();

            if (!ValidarFormulario())
                return;

            AbrirSistemaPrincipal();
        }

        /// <summary>
        /// Sincroniza los controles visuales con el ViewModel.
        ///
        /// PasswordBox no permite binding directo seguro,
        /// por eso se asigna manualmente.
        /// </summary>
        private void SincronizarViewModel()
        {
            _viewModel.CorreoInstitucional =
                TxtCorreoInstitucional.Text.Trim();

            _viewModel.Contrasena =
                TxtContrasena.Password.Trim();

            _viewModel.MantenerSesion =
                ChkMantenerSesion.IsChecked == true;
        }

        /// <summary>
        /// Valida que los campos obligatorios del login estén completos.
        ///
        /// Validaciones actuales:
        /// - Correo institucional obligatorio.
        /// - Contraseña obligatoria.
        ///
        /// </summary>
        /// <returns>
        /// true si el formulario es válido.
        /// false si falta algún campo obligatorio.
        /// </returns>
        private bool ValidarFormulario()
        {
            bool valido = true;

            if (!_viewModel.CorreoEsValido())
            {
                TxtErrorCorreo.Text = "ⓘ El usuario es obligatorio.";
                TxtErrorCorreo.Visibility = Visibility.Visible;
                valido = false;
            }

            if (!_viewModel.ContrasenaEsValida())
            {
                TxtErrorContrasena.Text = "ⓘ Contraseña obligatoria.";
                TxtErrorContrasena.Visibility = Visibility.Visible;
                valido = false;
            }
            else if (!_viewModel.ContrasenaCumpleLongitudMinima())
            {
                TxtErrorContrasena.Text = "ⓘ La contraseña debe tener mínimo 6 caracteres.";
                TxtErrorContrasena.Visibility = Visibility.Visible;
                valido = false;
            }

            return valido;
        }

        /// <summary>
        /// Carga MainShellView dentro de MainWindow.
        ///
        /// MainShellView mostrará DashboardView como pantalla inicial
        /// cuando la rama del Dashboard ya esté integrada en develop.
        /// </summary>
        private void AbrirSistemaPrincipal()
        {
            Window? ventanaPrincipal = Window.GetWindow(this);

            if (ventanaPrincipal == null)
                return;

            ventanaPrincipal.Content = new MainShellView();
        }

        /// <summary>
        /// Limpia todos los mensajes visuales del formulario.
        ///
        /// Oculta:
        /// - Error de correo.
        /// - Error de contraseña.
        /// - Error de credenciales.
        /// - Estado de conexión con API.
        /// </summary>
        private void LimpiarMensajes()
        {
            TxtErrorCorreo.Text = "";
            TxtErrorCorreo.Visibility = Visibility.Collapsed;

            TxtErrorContrasena.Text = "";
            TxtErrorContrasena.Visibility = Visibility.Collapsed;

            TxtErrorCredenciales.Visibility = Visibility.Collapsed;

            TxtEstadoApi.Text = "";
        }

        /// <summary>
        /// Evento ejecutado cuando cambia el texto del correo institucional.
        /// Actualiza la visibilidad del placeholder del campo de correo.
        /// </summary>
        private void TxtCorreoInstitucional_TextChanged(object sender, TextChangedEventArgs e)
        {
            ActualizarPlaceholders();
        }

        /// <summary>
        /// Evento ejecutado cuando cambia el texto de la contraseña.
        /// Actualiza la visibilidad del placeholder del campo de contraseña.
        /// </summary>
        private void TxtContrasena_PasswordChanged(object sender, RoutedEventArgs e)
        {
            ActualizarPlaceholders();
        }

        /// <summary>
        /// Muestra u oculta los placeholders personalizados de los campos.
        ///
        /// En WPF, PasswordBox no tiene Placeholder nativo,
        /// por eso se usa un TextBlock encima del campo y se oculta
        /// cuando el usuario escribe.
        /// </summary>
        private void ActualizarPlaceholders()
        {
            PlaceholderCorreo.Visibility =
                string.IsNullOrWhiteSpace(TxtCorreoInstitucional.Text)
                    ? Visibility.Visible
                    : Visibility.Collapsed;

            PlaceholderContrasena.Visibility =
                string.IsNullOrWhiteSpace(TxtContrasena.Password)
                    ? Visibility.Visible
                    : Visibility.Collapsed;
        }
    }
}