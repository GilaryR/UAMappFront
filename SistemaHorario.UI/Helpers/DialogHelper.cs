using SistemaHorario.UI.Dialogs.Shared;
using System.Windows;

namespace SistemaHorario.UI.Helpers
{
    /// <summary>
    /// Helper encargado de centralizar la apertura de diálogos
    /// reutilizables del sistema.
    ///
    /// Permite que las vistas usen una única clase para mostrar
    /// confirmaciones, mensajes de éxito, confirmaciones de eliminación
    /// y confirmaciones de cierre de sesión.
    ///
    /// Esto evita repetir la creación de ventanas en cada pantalla
    /// y facilita cambiar los diálogos en un solo lugar.
    /// </summary>
    public static class DialogHelper
    {
        /// <summary>
        /// Muestra un diálogo genérico de confirmación.
        ///
        /// Se utiliza para acciones como:
        /// - Crear registros.
        /// - Guardar cambios.
        /// - Aprobar procesos.
        /// - Confirmar acciones importantes.
        /// </summary>
        /// <param name="owner">
        /// Ventana dueña del diálogo.
        /// </param>
        /// <param name="titulo">
        /// Título principal del diálogo.
        /// </param>
        /// <param name="mensaje">
        /// Mensaje descriptivo para el usuario.
        /// </param>
        /// <returns>
        /// true si el usuario confirma.
        /// false si cancela.
        /// </returns>
        public static bool MostrarConfirmacion(
            Window owner,
            string titulo,
            string mensaje)
        {
            ConfirmacionDialog dialog = new(titulo, mensaje)
            {
                Owner = owner
            };

            return dialog.ShowDialog() == true;
        }

        /// <summary>
        /// Muestra un diálogo de operación exitosa.
        ///
        /// Se utiliza después de completar correctamente
        /// una acción del sistema.
        /// </summary>
        /// <param name="owner">
        /// Ventana dueña del diálogo.
        /// </param>
        /// <param name="titulo">
        /// Texto principal del mensaje de éxito.
        /// </param>
        public static void MostrarExito(
            Window owner,
            string titulo)
        {
            MensajeExitoDialog dialog = new(titulo)
            {
                Owner = owner
            };

            dialog.ShowDialog();
        }

        /// <summary>
        /// Muestra un diálogo de confirmación para eliminación.
        ///
        /// El usuario debe escribir la palabra requerida
        /// para habilitar la acción.
        /// </summary>
        /// <param name="owner">
        /// Ventana dueña del diálogo.
        /// </param>
        /// <param name="mensaje">
        /// Mensaje personalizado de eliminación.
        /// </param>
        /// <returns>
        /// true si el usuario confirma la eliminación.
        /// false si cancela.
        /// </returns>
        public static bool MostrarConfirmacionEliminar(
            Window owner,
            string mensaje = "¿Estás seguro que deseas eliminar?\nDebes escribir la palabra eliminar.")
        {
            EliminarConfirmacionDialog dialog = new(mensaje)
            {
                Owner = owner
            };

            return dialog.ShowDialog() == true;
        }

        /// <summary>
        /// Muestra el diálogo de confirmación de cierre de sesión.
        /// </summary>
        /// <param name="owner">
        /// Ventana dueña del diálogo.
        /// </param>
        /// <returns>
        /// true si el usuario confirma cerrar sesión.
        /// false si cancela.
        /// </returns>
        public static bool MostrarCerrarSesion(Window owner)
        {
            CerrarSesionDialog dialog = new()
            {
                Owner = owner
            };

            return dialog.ShowDialog() == true;
        }

        /// <summary>
        /// Muestra un mensaje informativo básico.
        ///
        /// Se conserva como respaldo temporal para casos donde
        /// aún no exista un diálogo personalizado específico.
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
        /// Muestra un mensaje de error básico.
        ///
        /// Se conserva usando MessageBox mientras se define
        /// si el sistema tendrá un diálogo personalizado de error.
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
    }
}