
namespace SistemaHorario.UI.Models.UI
{
    /// <summary>
    /// Modelo visual utilizado para representar una opción
    /// dentro del menú lateral de navegación.
    ///
    /// Este modelo no pertenece al backend ni a la base de datos.
    /// Su función es permitir que la SidebarView pueda construir
    /// opciones de menú dinámicas dependiendo del rol del usuario.
    /// </summary>
    public class MenuItemModel
    {
        /// <summary>
        /// Nombre visible de la opción en el menú.
        /// Ejemplo: Inicio, Materias, Docentes.
        /// </summary>
        public string Titulo { get; set; } = string.Empty;

        /// <summary>
        /// Ruta del icono que se mostrará junto al texto.
        /// Ejemplo: /Assets/Icons/IcInicio.png
        /// </summary>
        public string Icono { get; set; } = string.Empty;

        /// <summary>
        /// Nombre interno de la vista o módulo al que navega.
        /// Ejemplo: Dashboard, Materias, Reportes.
        /// </summary>
        public string VistaDestino { get; set; } = string.Empty;

        /// <summary>
        /// Indica si la opción está seleccionada actualmente.
        /// Sirve para pintar visualmente el botón activo.
        /// </summary>
        public bool Seleccionado { get; set; }
    }
}