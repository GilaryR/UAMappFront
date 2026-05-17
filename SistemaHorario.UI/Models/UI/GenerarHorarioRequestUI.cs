namespace SistemaHorario.UI.Models.UI
{
    /// <summary>
    /// Modelo utilizado para generar horarios.
    ///
    /// Actualmente funciona como modelo visual temporal.
    ///
    /// Más adelante este modelo será enviado al backend:
    ///
    /// POST /api/horarios/generar
    /// </summary>
    public class GenerarHorarioRequestUI
    {
        /// <summary>
        /// Grupo académico seleccionado.
        /// </summary>
        public int IdGrupo { get; set; }

        /// <summary>
        /// Hora inicial permitida para generación.
        /// </summary>
        public string HoraInicio { get; set; }
            = "07:00";

        /// <summary>
        /// Hora final permitida para generación.
        /// </summary>
        public string HoraFinal { get; set; }
            = "22:30";

        /// <summary>
        /// Duración del bloque académico.
        ///
        /// Ejemplo:
        /// - 1 hora
        /// - 2 horas
        /// </summary>
        public int DuracionBloque { get; set; }

        /// <summary>
        /// Días habilitados para generar horarios.
        /// </summary>
        public List<string> Dias { get; set; } = [];
    }
}