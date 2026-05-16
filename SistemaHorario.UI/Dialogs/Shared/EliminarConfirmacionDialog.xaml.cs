using System;
using System.Windows;
using System.Windows.Media;

namespace SistemaHorario.UI.Dialogs.Shared
{
    /// <summary>
    /// Diálogo reutilizable para confirmar eliminaciones.
    ///
    /// Para evitar eliminaciones accidentales, el usuario debe
    /// escribir la palabra "eliminar" antes de poder confirmar.
    ///
    /// Se utilizará en módulos como:
    /// - Materias.
    /// - Docentes.
    /// - Grupos académicos.
    /// - Coordinadores.
    /// - Reportes.
    /// </summary>
    public partial class EliminarConfirmacionDialog : Window
    {
        /// <summary>
        /// Palabra requerida para habilitar la eliminación.
        /// </summary>
        private const string PalabraConfirmacion = "eliminar";

        /// <summary>
        /// Inicializa el diálogo de eliminación.
        /// </summary>
        /// <param name="mensaje">
        /// Mensaje descriptivo personalizado.
        /// </param>
        public EliminarConfirmacionDialog(
            string mensaje = "¿Estás seguro que deseas eliminar?\nDebes escribir la palabra eliminar.")
        {
            InitializeComponent();

            TxtMensaje.Text = mensaje;
        }

        /// <summary>
        /// Cancela la eliminación y cierra el diálogo.
        /// </summary>
        private void BtnCancelar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        /// <summary>
        /// Confirma la eliminación y cierra el diálogo.
        /// </summary>
        private void BtnGuardar_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        /// <summary>
        /// Valida el texto escrito por el usuario.
        ///
        /// El botón de guardar solo se habilita cuando
        /// el texto ingresado es exactamente "eliminar".
        /// </summary>
        private void TxtConfirmacion_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            bool textoValido = TxtConfirmacion.Text.Trim()
                .Equals(PalabraConfirmacion, StringComparison.OrdinalIgnoreCase);

            TxtPlaceholder.Visibility =
                string.IsNullOrWhiteSpace(TxtConfirmacion.Text)
                    ? Visibility.Visible
                    : Visibility.Collapsed;

            BtnGuardar.IsEnabled = textoValido;
            BtnGuardar.Opacity = textoValido ? 1 : 0.55;
        }
    }
}