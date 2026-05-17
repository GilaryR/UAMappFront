using System;
using System.Windows;
using System.Windows.Controls;
using SistemaHorario.UI.Views.Auth;
using SistemaHorario.UI.Dialogs.Shared;


namespace SistemaHorario.UI.Views.Shell
{
    /// <summary>
    /// Menú desplegable del usuario autenticado.
    ///
    /// Esta vista se muestra cuando el usuario hace clic
    /// sobre su nombre o icono en la TopBar.
    ///
    /// Permite acceder a:
    /// - Mi perfil.
    /// - Cerrar sesión.
    ///
    /// Actualmente los datos del usuario son temporales.
    /// Más adelante serán cargados desde la sesión real,
    /// obtenida después del login.
    /// </summary>
    public partial class UserMenuView : UserControl
    {
        /// <summary>
        /// Evento que se ejecuta cuando el usuario selecciona
        /// la opción "Mi perfil".
        /// </summary>
        public event EventHandler? PerfilSolicitado;

        /// <summary>
        /// Evento que se ejecuta cuando el usuario selecciona
        /// la opción "Cerrar sesión".
        /// </summary>
        public event EventHandler? CerrarSesionSolicitado;

        /// <summary>
        /// Constructor del menú de usuario.
        ///
        /// Inicializa los componentes visuales y carga datos
        /// temporales mientras se conecta la autenticación real.
        /// </summary>
        public UserMenuView()
        {
            InitializeComponent();

            Visibility = Visibility.Collapsed;

            // TODO:
            // Reemplazar estos datos por la información real
            // del usuario autenticado.
            ConfigurarUsuario(
                "Administrador",
                "administrador@autonoma.edu.co"
            );
        }

        /// <summary>
        /// Configura la información mostrada dentro del menú.
        /// </summary>
        /// <param name="nombreCompleto">
        /// Nombre completo del usuario autenticado.
        /// </param>
        /// <param name="correoInstitucional">
        /// Correo institucional del usuario autenticado.
        /// </param>
        public void ConfigurarUsuario(
            string nombreCompleto,
            string correoInstitucional)
        {
            TxtNombreUsuario.Text = ObtenerNombreCorto(nombreCompleto);
            TxtCorreoUsuario.Text = correoInstitucional;
        }

        /// <summary>
        /// Muestra el menú de usuario.
        /// </summary>
        public void Mostrar()
        {
            Visibility = Visibility.Visible;
        }

        /// <summary>
        /// Oculta el menú de usuario.
        /// </summary>
        public void Ocultar()
        {
            Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Alterna la visibilidad del menú.
        ///
        /// Si está oculto, lo muestra.
        /// Si está visible, lo oculta.
        /// </summary>
        public void Alternar()
        {
            Visibility = Visibility == Visibility.Visible
                ? Visibility.Collapsed
                : Visibility.Visible;
        }

        /// <summary>
        /// Obtiene un nombre corto para mostrar en la interfaz.
        ///
        /// Ejemplo:
        /// "Juan Felipe Pérez Gómez" se muestra como "Juan Felipe".
        ///
        /// Si después quieres mostrar primer nombre + primer apellido,
        /// esta lógica se puede ajustar cuando el backend entregue
        /// la estructura final del nombre.
        /// </summary>
        /// <param name="nombreCompleto">
        /// Nombre completo del usuario.
        /// </param>
        /// <returns>
        /// Nombre simplificado para la UI.
        /// </returns>
        private string ObtenerNombreCorto(string nombreCompleto)
        {
            if (string.IsNullOrWhiteSpace(nombreCompleto))
                return "Usuario";

            string[] partes = nombreCompleto.Split(
                ' ',
                StringSplitOptions.RemoveEmptyEntries
            );

            if (partes.Length == 1)
                return partes[0];

            return $"{partes[0]} {partes[1]}";
        }

        /// <summary>
        /// Evento ejecutado al presionar la opción "Mi perfil".
        ///
        /// Notifica al contenedor principal para navegar
        /// hacia la vista de perfil.
        /// </summary>
        private void BtnMiPerfil_Click(object sender, RoutedEventArgs e)
        {
            Ocultar();
            PerfilSolicitado?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Evento ejecutado al presionar la opción "Cerrar sesión".
        ///
        /// Notifica al contenedor principal para cerrar la sesión
        /// y volver al login.
        /// </summary>
        private void BtnCerrarSesion_Click(object sender, RoutedEventArgs e)
        {
            CerrarSesionDialog dialog = new CerrarSesionDialog
            {
                Owner = Window.GetWindow(this)
            };

            bool? resultado = dialog.ShowDialog();

            if (resultado != true)
                return;

            CerrarSesion();
        }

        /// <summary>
        /// Cierra la sesión visual actual y retorna al LoginView.
        ///
        /// TODO:
        /// Cuando exista autenticación real, limpiar aquí UsuarioSesion,
        /// token JWT y cualquier dato persistido.
        /// </summary>
        private void CerrarSesion()
        {
            Window? ventanaPrincipal = Window.GetWindow(this);

            if (ventanaPrincipal == null)
                return;

            ventanaPrincipal.Content = new LoginView();
        }
    }
}