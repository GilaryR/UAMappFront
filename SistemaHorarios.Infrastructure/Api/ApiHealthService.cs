using SistemaHorarios.Application.Common;
using SistemaHorarios.Application.Common.Auth;
using System.IO;
using System.Net.Http.Json;
using System.Text.Json;

namespace SistemaHorario.Infrastructure.Api;

public class AuthApiService
{
    private const string BaseUrlPorDefecto = "http://localhost:5023/api/";

    private readonly HttpClient _httpClient;

    public AuthApiService()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(ObtenerBaseUrl())
        };
    }

    private static string ObtenerBaseUrl()
    {
        var rutaConfiguracion =
            Path.Combine(AppContext.BaseDirectory, "appsettings.json");

        if (!File.Exists(rutaConfiguracion))
        {
            return BaseUrlPorDefecto;
        }

        try
        {
            var contenido = File.ReadAllText(rutaConfiguracion);
            using var documento = JsonDocument.Parse(contenido);

            if (!documento.RootElement.TryGetProperty("ApiSettings", out JsonElement apiSettings))
            {
                return BaseUrlPorDefecto;
            }

            if (!apiSettings.TryGetProperty("BaseUrl", out JsonElement baseUrlElemento))
            {
                return BaseUrlPorDefecto;
            }

            var baseUrl = baseUrlElemento.GetString();

            if (string.IsNullOrWhiteSpace(baseUrl))
            {
                return BaseUrlPorDefecto;
            }

            return baseUrl.EndsWith('/') ? baseUrl : baseUrl + "/";
        }
        catch
        {
            return BaseUrlPorDefecto;
        }
    }

    public async Task<ApiResponse<LoginData>> LoginAsync(LoginRequest request)
    {
        try
        {
            HttpResponseMessage response =
                await _httpClient.PostAsJsonAsync(
                    "auth/login",
                    request
                );

            ApiResponse<LoginData>? apiResponse =
                await response.Content.ReadFromJsonAsync<ApiResponse<LoginData>>();

            if (apiResponse is not null)
                return apiResponse;

            return new ApiResponse<LoginData>
            {
                Success = false,
                Message = "La API no devolvió una respuesta válida."
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<LoginData>
            {
                Success = false,
                Message = $"No se pudo conectar con la API: {ex.Message}"
            };
        }
    }
}