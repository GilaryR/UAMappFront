namespace SistemaHorarios.Application.Common
{
    /// <summary>
    /// Respuesta simple usada por servicios internos del frontend.
    /// </summary>
    public class ApiResponse
    {
        public bool Exitoso { get; set; }

        public string Mensaje { get; set; } = string.Empty;

        public bool Success
        {
            get => Exitoso;
            set => Exitoso = value;
        }

        public string Message
        {
            get => Mensaje;
            set => Mensaje = value;
        }
    }

    /// <summary>
    /// Respuesta genérica usada para mapear respuestas reales del backend.
    /// El backend responde con:
    /// success, message y data.
    /// </summary>
    public class ApiResponse<T>
    {
        public bool Success { get; set; }

        public string Message { get; set; } = string.Empty;

        public T? Data { get; set; }

        public bool Exitoso => Success;

        public string Mensaje => Message;
    }
}