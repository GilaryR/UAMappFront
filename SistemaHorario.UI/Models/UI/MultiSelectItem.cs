

namespace SistemaHorario.UI.Models.UI
{
    /// <summary>
    /// Representa una opción seleccionable dentro del control MultiSelectDropdown.
    ///
    /// Se usa para listas dinámicas como:
    /// - Materias
    /// - Días
    /// - Docentes
    /// - Prerrequisitos
    /// </summary>
    public class MultiSelectItem
    {
        /// <summary>
        /// Identificador del elemento.
        /// Puede venir desde la API.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Texto visible que se muestra al usuario.
        /// </summary>
        public string Texto { get; set; } = string.Empty;

        /// <summary>
        /// Indica si el elemento está seleccionado.
        /// </summary>
        public bool Seleccionado { get; set; }
    }
}