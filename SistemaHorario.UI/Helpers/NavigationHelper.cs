using System.Windows.Controls;

namespace SistemaHorario.UI.Helpers
{
    /// <summary>
    /// Helper encargado de centralizar la navegación visual
    /// dentro de la aplicación.
    ///
    /// Permite cargar vistas dentro de un ContentControl,
    /// evitando repetir la misma lógica en diferentes partes
    /// del sistema.
    ///
    /// Este helper no consume API ni contiene lógica de negocio.
    /// Su responsabilidad es únicamente cambiar el contenido visual.
    /// </summary>
    public static class NavigationHelper
    {
        /// <summary>
        /// Carga una vista dentro de un contenedor visual.
        ///
        /// Ejemplo de uso:
        /// NavigationHelper.CargarVista(ContentArea, new MateriasView());
        /// </summary>
        /// <param name="contenedor">
        /// ContentControl donde se mostrará la vista.
        /// </param>
        /// <param name="vista">
        /// Vista que se desea cargar dentro del contenedor.
        /// </param>
        public static void CargarVista(
            ContentControl contenedor,
            UserControl vista)
        {
            contenedor.Content = vista;
        }

        /// <summary>
        /// Limpia el contenido actual de un contenedor visual.
        ///
        /// Se puede usar al cerrar sesión o al reiniciar
        /// una zona de navegación.
        /// </summary>
        /// <param name="contenedor">
        /// ContentControl que será limpiado.
        /// </param>
        public static void LimpiarVista(ContentControl contenedor)
        {
            contenedor.Content = null;
        }
    }
}