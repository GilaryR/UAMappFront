
namespace SistemaHorario.UI.Models.UI
{
    /// <summary>
    /// Representa una materia dentro de la interfaz gráfica.
    ///
    /// Este modelo pertenece únicamente a la capa UI.
    /// No es un DTO de API.
    ///
    /// Se usa para:
    /// - Mostrar materias en la tabla.
    /// - Cargar datos en el dialog de crear, editar o ver.
    /// - Aplicar filtros visuales.
    ///
    /// Más adelante, cuando se conecte la API, el servicio deberá
    /// mapear la respuesta de /api/materias hacia este modelo.
    /// </summary>
    public class MateriaItem
    {
        /// <summary>
        /// Identificador de la materia.
        /// Corresponde a idMateria en la respuesta de API.
        /// </summary>
        public int IdMateria { get; set; }

        /// <summary>
        /// Código académico de la materia.
        /// </summary>
        public string Codigo { get; set; } = string.Empty;

        /// <summary>
        /// Nombre de la materia.
        /// </summary>
        public string Nombre { get; set; } = string.Empty;

        /// <summary>
        /// Créditos académicos de la materia.
        /// </summary>
        public int Creditos { get; set; }

        /// <summary>
        /// Intensidad horaria semanal.
        /// </summary>
        public int IntensidadHorariaSemanal { get; set; }

        /// <summary>
        /// Semestre al que pertenece la materia.
        /// </summary>
        public int Semestre { get; set; }

        /// <summary>
        /// Cantidad de grupos asociados a la materia.
        ///
        /// NOTA:
        /// Este dato aún no está en el endpoint actual,
        /// pero se deja preparado para integración futura.
        /// </summary>
        public int CantidadGrupos { get; set; }

        /// <summary>
        /// Indica si la materia está activa.
        /// </summary>
        public bool Activa { get; set; }

        /// <summary>
        /// Texto visible del estado.
        /// </summary>
        public string EstadoTexto => Activa ? "Activa" : "Inactiva";
    }
}