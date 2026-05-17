namespace SistemaHorario.UI.Models.UI
{
    /// <summary>
    /// Modelo visual que representa una asignatura ubicada
    /// dentro de una celda del horario semanal.
    ///
    /// Este modelo será usado por VistaPreviaHorarioView.
    ///
    /// Más adelante se mapeará desde:
    /// GET /api/horarios/{id}/vista-previa
    /// </summary>
    public class BloqueHorarioItem
    {
        public int IdHorario { get; set; }

        public int IdMateria { get; set; }

        public int IdDocente { get; set; }

        public string Dia { get; set; } = string.Empty;

        public string HoraInicio { get; set; } = string.Empty;

        public string HoraFinal { get; set; } = string.Empty;

        public string Materia { get; set; } = string.Empty;

        public string Docente { get; set; } = string.Empty;

        public string Aula { get; set; } = string.Empty;

        public string Modalidad { get; set; } = string.Empty;

        public string ColorVisual { get; set; } = "#B9EFC2";

        public string TextoCelda =>
            $"{Materia}\n{Docente}\n{Aula}\n{Modalidad}";
    }
}