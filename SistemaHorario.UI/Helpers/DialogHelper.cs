using System.Windows;

namespace SistemaHorario.UI.Helpers
{
    /// <summary>
    /// Helper encargado de centralizar los mensajes
    /// y confirmaciones visuales del sistema.
    ///
    /// Actualmente utiliza MessageBox de WPF como implementación
    /// temporal.
    ///
    /// Más adelante, cuando se creen los diálogos personalizados
    /// en Dialogs/Shared, este helper podrá modificarse para usar:
    /// - ConfirmacionDialog
    /// - MensajeExitoDialog
    /// - EliminarConfirmacionDialog
    /// - CerrarSesionDialog
    ///
    /// De esta forma, las vistas no tendrán que cambiar su lógica.
    /// Solo se actualizará este helper.
    /// </summary>
    public static class DialogHelper
    {
        /// <summary>
        /// Muestra un mensaje informativo al usuario.
        /// </summary>
        public static void MostrarInformacion(
            string mensaje,
            string titulo = "Información")
        {
            MessageBox.Show(
                mensaje,
                titulo,
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }

        /// <summary>
        /// Muestra un mensaje de operación exitosa.
        /// </summary>
        public static void MostrarExito(
            string mensaje,
            string titulo = "Operación exitosa")
        {
            MessageBox.Show(
                mensaje,
                titulo,
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );
        }

        /// <summary>
        /// Muestra un mensaje de error al usuario.
        /// </summary>
        public static void MostrarError(
            string mensaje,
            string titulo = "Error")
        {
            MessageBox.Show(
                mensaje,
                titulo,
                MessageBoxButton.OK,
                MessageBoxImage.Error
            );
        }

        /// <summary>
        /// Muestra una ventana de confirmación.
        ///
        /// Retorna true si el usuario confirma.
        /// Retorna false si el usuario cancela.
        /// </summary>
        public static bool Confirmar(
            string mensaje,
            string titulo = "Confirmación")
        {
            MessageBoxResult resultado = MessageBox.Show(
                mensaje,
                titulo,
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            return resultado == MessageBoxResult.Yes;
        }
    }
}