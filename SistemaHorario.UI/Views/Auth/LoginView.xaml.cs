using SistemaHorario.Infrastructure.Api;
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
    ///
    /// NOTA:
    /// En esta etapa todavía no se consume el endpoint real de login.
    /// El formulario queda preparado para que posteriormente se conecte
    /// con /api/auth/login.
    /// </summary>
    public partial class LoginView : UserControl
    {
        /// <summary>
        /// Servicio utilizado para validar la conexión con la API.
        /// </summary>
        private readonly ApiHealthService _apiHealthService;

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

            _apiHealthService = new ApiHealthService();

            TxtCorreoInstitucional.TextChanged += TxtCorreoInstitucional_TextChanged;
            TxtContrasena.PasswordChanged += TxtContrasena_PasswordChanged;

            ActualizarPlaceholders();
        }

        /// <summary>
        /// Evento ejecutado al presionar el botón "Iniciar sesión".
        ///
        /// Flujo actual:
        /// 1. Limpia mensajes previos.
        /// 2. Valida campos obligatorios.
        /// 3. Si los campos son válidos, prueba la conexión con la API.
        /// 4. Muestra el resultado de la conexión.
        ///
        /// Este método queda preparado para que en el futuro
        /// se reemplace la prueba GET por el consumo real del endpoint:
        /// POST /api/auth/login.
        /// </summary>
        private async void BtnIniciarSesion_Click(object sender, RoutedEventArgs e)
        {
            LimpiarMensajes();

            bool formularioValido = ValidarFormulario();

            if (!formularioValido)
                return;

            BtnIniciarSesion.IsEnabled = false;
            TxtEstadoApi.Text = "Probando conexión con la API...";
            TxtEstadoApi.Foreground = Brushes.DarkOrange;

            ApiResponse respuesta = await _apiHealthService.ProbarConexionAsync();

            BtnIniciarSesion.IsEnabled = true;

            if (respuesta.Exitoso)
            {
                TxtEstadoApi.Text = respuesta.Mensaje;
                TxtEstadoApi.Foreground = Brushes.Green;

                MessageBox.Show(
                    "Login preparado correctamente. La autenticación real se conectará después.",
                    "Login preparado",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );

                return;
            }

            TxtEstadoApi.Text = respuesta.Mensaje;
            TxtEstadoApi.Foreground = Brushes.Red;
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

            string correoInstitucional = TxtCorreoInstitucional.Text.Trim();
            string contrasena = TxtContrasena.Password.Trim();

            if (string.IsNullOrWhiteSpace(correoInstitucional))
            {
                TxtErrorCorreo.Text = "ⓘ El usuario es obligatorio.";
                TxtErrorCorreo.Visibility = Visibility.Visible;
                valido = false;
            }

            if (string.IsNullOrWhiteSpace(contrasena))
            {
                TxtErrorContrasena.Text = "ⓘ Contraseña obligatoria";
                TxtErrorContrasena.Visibility = Visibility.Visible;
                valido = false;
            }

            return valido;
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