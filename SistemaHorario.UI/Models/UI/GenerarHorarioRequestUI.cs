using System.Collections.Generic;

namespace SistemaHorario.UI.Models.UI
{
    /// <summary>
    /// Modelo preparado para solicitar la generación automática
    /// de horarios según el contrato actual documentado del backend.
    ///
    /// Endpoint:
    /// POST /api/horarios/generar
    ///
    /// Contrato esperado por backend:
    ///
    /// {
    ///   "horaInicio": "07:00",
    ///   "horaFinal": "22:30",
    ///   "duracionBloque": 60,
    ///   "dias": ["Lunes", "Martes"]
    /// }
    ///
    /// Nota:
    /// Actualmente el backend NO recibe IdGrupo.
    /// Si se necesita generar horario por grupo académico,
    /// backend debe agregar IdGrupo al request.
    /// </summary>
    public class GenerarHorarioRequestUI
    {
        /// <summary>
        /// Hora inicial permitida para la generación.
        /// Formato esperado: HH:mm.
        /// </summary>
        public string HoraInicio { get; set; } = "07:00";

        /// <summary>
        /// Hora final permitida para la generación.
        /// Formato esperado: HH:mm.
        /// </summary>
        public string HoraFinal { get; set; } = "22:30";

        /// <summary>
        /// Duración del bloque en minutos.
        ///
        /// Ejemplo:
        /// 60 = una hora.
        /// 120 = dos horas.
        /// </summary>
        public int DuracionBloque { get; set; } = 60;

        /// <summary>
        /// Días habilitados para generar el horario.
        /// </summary>
        public List<string> Dias { get; set; } = new();
    }
}