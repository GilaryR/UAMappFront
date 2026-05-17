namespace SistemaHorario.UI.Models.UI
{
    /// <summary>
    /// Modelo visual utilizado para representar
    /// los grupos académicos disponibles al momento
    /// de generar horarios.
    ///
    /// Este modelo será llenado más adelante desde:
    ///
    /// GET /api/grupos/activos
    ///
    /// La intención es que el usuario seleccione
    /// un grupo antes de generar un horario.
    /// </summary>
    public class GrupoHorarioOption
    {
        /// <summary>
        /// Identificador único del grupo.
        /// </summary>
        public int IdGrupo { get; set; }

        /// <summary>
        /// Nombre visual mostrado en el ComboBox.
        ///
        /// Ejemplo:
        /// - "Grupo 1 - Ingeniería"
        /// - "Grupo TAPSI Nocturno"
        /// </summary>
        public string NombreGrupo { get; set; }
            = string.Empty;

        /// <summary>
        /// Semestre académico asociado.
        /// </summary>
        public int Semestre { get; set; }

        /// <summary>
        /// Jornada del grupo.
        ///
        /// Ejemplo:
        /// - Diurna
        /// - Nocturna
        /// </summary>
        public string Jornada { get; set; }
            = string.Empty;

        /// <summary>
        /// Texto final mostrado visualmente
        /// dentro del ComboBox.
        /// </summary>
        public string DisplayTexto =>
            $"{NombreGrupo} | Sem {Semestre} | {Jornada}";
    }
}