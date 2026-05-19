namespace SistemaHorario.UI.Models.UI
{
    /// <summary>
    /// Modelo visual que representa un horario generado
    /// dentro de la tabla principal del módulo Horarios.
    ///
    /// Este modelo será usado por HorariosView.
    ///
    /// Más adelante se mapeará desde:
    /// GET /api/horarios
    /// </summary>
    public class HorarioItem
    {
        /// <summary>
        /// Identificador único del horario.
        /// </summary>
        public int IdHorario { get; set; }

        /// <summary>
        /// Nombre visual del horario generado.
        /// </summary>
        public string Nombre { get; set; } = string.Empty;

        /// <summary>
        /// Nombre del grupo académico al que pertenece el horario.
        /// </summary>
        public string Grupo { get; set; } = string.Empty;

        /// <summary>
        /// Tipo de grupo o estudiante.
        ///
        /// Ejemplo:
        /// Regular o TAPSI.
        /// </summary>
        public string Tipo { get; set; } = string.Empty;

        /// <summary>
        /// Jornada del horario.
        ///
        /// Ejemplo:
        /// Diurna o Nocturna.
        /// </summary>
        public string Jornada { get; set; } = string.Empty;

        /// <summary>
        /// Fecha de generación del horario.
        /// </summary>
        public string FechaGeneracion { get; set; } = string.Empty;

        /// <summary>
        /// Estado actual del horario.
        ///
        /// Ejemplo:
        /// Pendiente, Aprobado o Rechazado.
        /// </summary>
        public string Estado { get; set; } = string.Empty;

        public int IdGrupo { get; set; }
    }
}