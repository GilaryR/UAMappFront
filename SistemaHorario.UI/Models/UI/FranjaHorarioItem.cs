namespace SistemaHorario.UI.Models.UI
{
    /// <summary>
    /// Modelo visual de una franja horaria institucional.
    /// </summary>
    public class FranjaHorarioItem
    {
        public int IdFranjaHoraria { get; set; }

        public string DiaSemana { get; set; } = string.Empty;

        public string HoraInicio { get; set; } = string.Empty;

        public string HoraFin { get; set; } = string.Empty;

        public bool Activa { get; set; }

        public string RangoTexto => $"{HoraInicio} - {HoraFin}";
    }
}
