namespace SistemaHorario.UI.Models.UI
{
    /// <summary>
    /// Modelo utilizado para rechazar un horario generado.
    ///
    /// Este modelo será enviado más adelante al endpoint:
    /// POST /api/horarios/{id}/rechazar
    ///
    /// Permite registrar el motivo académico o administrativo
    /// por el cual el horario fue rechazado antes de aprobarse.
    /// </summary>
    public class RechazarHorarioRequestUI
    {
        /// <summary>
        /// Identificador único del horario rechazado.
        /// </summary>
        public int IdHorario { get; set; }

        /// <summary>
        /// Motivo del rechazo registrado por coordinación.
        ///
        /// Ejemplos:
        /// - Cruce de docentes.
        /// - Exceso de carga académica.
        /// - Conflicto de aulas.
        /// - Solape entre grupos.
        /// </summary>
        public string MotivoRechazo { get; set; }
            = string.Empty;
    }
}