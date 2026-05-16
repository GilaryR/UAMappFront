using System.Windows;

namespace SistemaHorario.UI.Dialogs.Shared
{
    /// <summary>
    /// Diálogo reutilizable de confirmación.
    ///
    /// Se utiliza cuando el sistema necesita que el usuario confirme
    /// una acción antes de ejecutarla.
    ///
    /// Ejemplos:
    /// - Crear docente.
    /// - Guardar cambios.
    /// - Aprobar horario.
    /// - Confirmar una operación importante.
    /// </summary>
    public partial class ConfirmacionDialog : Window
    {
        /// <summary>
        /// Inicializa el diálogo con título y mensaje personalizados.
        /// </summary>
        /// <param name="titulo">
        /// Título principal del diálogo.
        /// </param>
        /// <param name="mensaje">
        /// Mensaje descriptivo mostrado al usuario.
        /// </param>
        public ConfirmacionDialog(string titulo, string mensaje)
        {
            InitializeComponent();

            TxtTitulo.Text = titulo;
            TxtMensaje.Text = mensaje;
        }

        /// <summary>
        /// Cancela la operación y cierra el diálogo.
        /// </summary>
        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        /// <summary>
        /// Confirma la operación y cierra el diálogo.
        /// </summary>
        private void BtnConfirmar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}