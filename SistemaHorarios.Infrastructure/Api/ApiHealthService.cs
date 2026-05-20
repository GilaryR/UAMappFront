using SistemaHorarios.Application.Common;
using SistemaHorarios.Application.Common.Auth;
using System.Net.Http.Json;
using System.Text.Json;

namespace SistemaHorario.Infrastructure.Api;

public class AuthApiService
{
    private readonly HttpClient _httpClient;

    public AuthApiService()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = ApiConfiguration.BaseUri
        };
    }

    public async Task<ApiResponse<LoginData>> LoginAsync(
        LoginRequest request)
    {
        try
        {
            HttpResponseMessage response =
                await _httpClient.PostAsJsonAsync(
                    "auth/login",
                    request
                );

            string contenido = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return new ApiResponse<LoginData>
                {
                    Success = false,
                    Message = ExtraerMensaje(contenido, response.ReasonPhrase)
                };
            }

            ApiResponse<LoginData>? apiResponse =
                JsonSerializer.Deserialize<ApiResponse<LoginData>>(
                    contenido,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );

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

    private static string ExtraerMensaje(string contenido, string? fallback)
    {
        if (string.IsNullOrWhiteSpace(contenido))
            return fallback ?? "No se pudo iniciar sesión.";

        try
        {
            using JsonDocument document = JsonDocument.Parse(contenido);
            JsonElement root = document.RootElement;

            if (root.ValueKind == JsonValueKind.String)
                return root.GetString() ?? string.Empty;

            if (root.ValueKind == JsonValueKind.Object)
            {
                foreach (JsonProperty property in root.EnumerateObject())
                {
                    if (string.Equals(property.Name, "message", StringComparison.OrdinalIgnoreCase)
                        || string.Equals(property.Name, "mensaje", StringComparison.OrdinalIgnoreCase))
                    {
                        return property.Value.ToString();
                    }
                }
            }
        }
        catch (JsonException)
        {
            return contenido;
        }

        return contenido;
    }
}
