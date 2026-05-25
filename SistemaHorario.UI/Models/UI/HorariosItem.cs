namespace SistemaHorario.UI.Models.UI
{
    /// <summary>
    /// Modelo visual que representa un horario de grupo o docente.
    /// </summary>
    public class HorarioItem
    {
        public int IdHorario { get; set; }

        public int IdGrupo { get; set; }

        public int IdDocente { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public string Grupo { get; set; } = string.Empty;

        public string Tipo { get; set; } = string.Empty;

        public string Jornada { get; set; } = string.Empty;

        public string FechaGeneracion { get; set; } = string.Empty;

        public string Estado { get; set; } = string.Empty;

        public string MotivoRechazo { get; set; } = string.Empty;

        public int NumeroSemestre { get; set; }

        public int CantidadBloques { get; set; }

        public string SemestreTexto => NumeroSemestre > 0
            ? $"Semestre {NumeroSemestre}"
            : "No definido";

        public string BloquesTexto => CantidadBloques == 1
            ? "1 bloque"
            : $"{CantidadBloques} bloques";

        public bool EsHorarioDocente { get; set; }
    }
}
