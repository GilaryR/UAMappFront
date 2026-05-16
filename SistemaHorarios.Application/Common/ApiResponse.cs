namespace SistemaHorarios.Application.Common
{
    /// <summary>
    /// Clase genérica utilizada para representar
    /// respuestas simples provenientes de servicios
    /// o peticiones HTTP hacia la API.
    ///
    /// Esta clase permite estandarizar la información
    /// devuelta por la capa Infrastructure hacia la UI.
    ///
    /// Ejemplos de uso:
    /// - Verificar conexión con la API.
    /// - Mostrar mensajes de éxito o error.
    /// - Validar operaciones simples.
    /// </summary>
    public class ApiResponse
    {
        /// <summary>
        /// Indica si la operación fue exitosa.
        ///
        /// true  = operación realizada correctamente.
        /// false = ocurrió un error.
        /// </summary>
        public bool Exitoso { get; set; }

        /// <summary>
        /// Mensaje descriptivo del resultado de la operación.
        ///
        /// Ejemplos:
        /// - "Conexión exitosa con la API."
        /// - "No se pudo conectar con la API."
        /// - "Credenciales inválidas."
        /// </summary>
        public string Mensaje { get; set; } = string.Empty;
    }
}
