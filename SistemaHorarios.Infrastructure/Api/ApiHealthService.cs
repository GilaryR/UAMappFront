using SistemaHorarios.Application.Common;
using System.Net.Http;

namespace SistemaHorario.Infrastructure.Api;

/// <summary>
/// Servicio encargado de verificar la conexión entre la aplicación WPF
/// y la API REST del sistema de horarios.
///
/// Este servicio realiza una petición GET a un endpoint existente
/// para validar:
/// 
/// - Que la API esté encendida.
/// - Que la URL base sea correcta.
/// - Que el backend responda correctamente.
/// - Que exista comunicación HTTP entre frontend y backend.
/// 
/// NOTA:
/// Actualmente este servicio solo se utiliza para pruebas iniciales
/// de conectividad.
/// </summary>
public class ApiHealthService
{
    /// <summary>
    /// Cliente HTTP utilizado para realizar peticiones a la API.
    /// </summary>
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Constructor del servicio.
    ///
    /// Inicializa el HttpClient y configura la URL base
    /// del backend.
    ///
    /// </summary>
    public ApiHealthService()
    {
        _httpClient = new HttpClient
        {
            // URL base del backend
            BaseAddress = new Uri("https://localhost:7208/")
        };
    }

    /// <summary>
    /// Realiza una prueba de conexión contra la API.
    ///
    /// Este método consume el endpoint:
    /// GET /api/dashboard/resumen
    ///
    /// Si la API responde correctamente (200 OK),
    /// se retorna una respuesta exitosa.
    ///
    /// Si ocurre un error de conexión o la API responde
    /// con un código diferente de éxito, se retorna
    /// una respuesta fallida.
    /// </summary>
    /// <returns>
    /// Un objeto ApiResponse con:
    /// 
    /// Exitoso:
    /// - true  -> conexión exitosa.
    /// - false -> error de conexión.
    ///
    /// Mensaje:
    /// - descripción del resultado.
    /// </returns>
    public async Task<ApiResponse> ProbarConexionAsync()
    {
        try
        {
            // Realiza petición GET al endpoint de prueba
            HttpResponseMessage response =
                await _httpClient.GetAsync("api/dashboard/resumen");

            // Verifica si la respuesta HTTP fue exitosa (200-299)
            if (response.IsSuccessStatusCode)
            {
                return new ApiResponse
                {
                    Exitoso = true,
                    Mensaje = "Conexión exitosa con la API."
                };
            }

            // La API respondió pero con error HTTP
            return new ApiResponse
            {
                Exitoso = false,
                Mensaje = $"La API respondió con estado {(int)response.StatusCode}."
            };
        }
        catch (Exception ex)
        {
            // Error general de conexión:
            // - API apagada
            // - URL incorrecta
            // - SSL inválido
            // - Sin internet/red
            // - Timeout
            return new ApiResponse
            {
                Exitoso = false,
                Mensaje = $"No se pudo conectar con la API: {ex.Message}"
            };
        }
    }
}