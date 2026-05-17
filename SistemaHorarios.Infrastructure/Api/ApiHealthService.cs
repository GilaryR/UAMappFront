using SistemaHorarios.Application.Common;
using SistemaHorarios.Application.Common.Auth;
using System.Net.Http.Json;

namespace SistemaHorario.Infrastructure.Api;

public class AuthApiService
{
    private readonly HttpClient _httpClient;

    public AuthApiService()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("http://localhost:5023/api/")
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