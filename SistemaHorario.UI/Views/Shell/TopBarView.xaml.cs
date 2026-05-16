using System;
using System.Windows.Controls;
using System.Windows.Input;

namespace SistemaHorario.UI.Views.Shell
{
    /// <summary>
    /// Barra superior principal del sistema.
    ///
    /// Esta vista se encarga de mostrar:
    /// - Nombre del sistema.
    /// - Usuario autenticado.
    /// - Imagen de usuario.
    /// - Botón desplegable del menú de usuario.
    ///
    /// La información mostrada actualmente es temporal
    /// mientras se implementa la autenticación real.
    ///
    /// Más adelante:
    /// - El nombre vendrá desde sesión/JWT.
    /// - La imagen podrá venir desde base de datos.
    /// - El menú se conectará con acciones reales.
    /// </summary>
    public partial class TopBarView : UserControl
    {
        /// <summary>
        /// Evento ejecutado cuando el usuario presiona
        /// el área del perfil.
        ///
        /// Será utilizado para abrir/cerrar UserMenuView.
        /// </summary>
        public event EventHandler? MenuUsuarioSolicitado;

        /// <summary>
        /// Constructor de TopBarView.
        ///
        /// Inicializa componentes visuales y carga
        /// datos temporales de prueba.
        /// </summary>
        public TopBarView()
        {
            InitializeComponent();

            // TODO:
            // Reemplazar por datos reales del usuario autenticado.
            ConfigurarUsuario(
                "Administrador",
                "administrador@uam.edu.co"
            );
        }

        /// <summary>
        /// Configura visualmente la información
        /// del usuario autenticado.
        ///
        /// Actualmente solo se muestra:
        /// - Primer nombre.
        /// - Primer apellido.
        ///
        /// Ejemplo:
        /// "Juan Pérez"
        /// </summary>
        /// <param name="nombreCompleto">
        /// Nombre completo del usuario.
        /// </param>
        /// <param name="correo">
        /// Correo institucional del usuario.
        /// </param>
        public void ConfigurarUsuario(
            string nombreCompleto,
            string correo)
        {
            TxtNombreUsuario.Text =
                ObtenerNombreCorto(nombreCompleto);
        }

        /// <summary>
        /// Genera un nombre corto utilizando:
        /// - Primer nombre.
        /// - Primer apellido.
        ///
        /// Ejemplo:
        /// "Juan Felipe Pérez Gómez"
        /// → "Juan Pérez"
        /// </summary>
        /// <param name="nombreCompleto">
        /// Nombre completo del usuario.
        /// </param>
        /// <returns>
        /// Nombre simplificado para interfaz gráfica.
        /// </returns>
        private string ObtenerNombreCorto(string nombreCompleto)
        {
            if (string.IsNullOrWhiteSpace(nombreCompleto))
                return "Usuario";

            string[] partes =
                nombreCompleto.Split(
                    ' ',
                    StringSplitOptions.RemoveEmptyEntries
                );

            if (partes.Length == 1)
                return partes[0];

            if (partes.Length >= 2)
                return $"{partes[0]} {partes[1]}";

            return nombreCompleto;
        }

        /// <summary>
        /// Evento ejecutado cuando el usuario
        /// hace clic sobre el perfil superior.
        ///
        /// Este evento notificará al MainShellView
        /// para mostrar u ocultar el UserMenuView.
        /// </summary>
        private void BtnUsuario_MouseLeftButtonUp(
            object sender,
            MouseButtonEventArgs e)
        {
            MenuUsuarioSolicitado?.Invoke(this, EventArgs.Empty);
        }
    }
}