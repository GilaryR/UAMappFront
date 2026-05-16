
namespace SistemaHorario.UI.Models.UI
{
    /// <summary>
    /// Representa una opción usada dentro de los filtros visuales.
    ///
    /// Ejemplos:
    /// - Estado: Activo
    /// - Semestre: 1
    /// - Rol: Administrador
    /// - Tipo reporte: PDF
    /// </summary>
    public class FilterOptionItem
    {
        /// <summary>
        /// Texto visible para el usuario.
        /// </summary>
        public string Label { get; set; } = string.Empty;

        /// <summary>
        /// Valor que se enviará después como parámetro al backend.
        /// </summary>
        public string Value { get; set; } = string.Empty;
    }
}