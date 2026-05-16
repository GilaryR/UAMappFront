using System.Windows;

namespace SistemaHorario.UI.Dialogs.Shared
{
    /// <summary>
    /// Diálogo reutilizable para confirmar el cierre de sesión.
    ///
    /// Se muestra cuando el usuario selecciona la opción
    /// "Cerrar sesión" desde el menú de usuario.
    ///
    /// Este diálogo no limpia la sesión directamente.
    /// Solo retorna una respuesta para que el contenedor principal
    /// o el ViewModel ejecuten la acción correspondiente.
    /// </summary>
    public partial class CerrarSesionDialog : Window
    {
        /// <summary>
        /// Constructor del diálogo de cierre de sesión.
        /// </summary>
        public CerrarSesionDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Cancela el cierre de sesión.
        /// </summary>
        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        /// <summary>
        /// Confirma el cierre de sesión.
        /// </summary>
        private void BtnCerrarSesion_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}