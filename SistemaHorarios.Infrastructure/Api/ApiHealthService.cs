using SistemaHorarios.Application.Common;
using SistemaHorarios.Application.Common.Auth;
using System.Net.Http.Json;

namespace SistemaHorario.Infrastructure.Api;

public class AuthApiService
{
    private static readonly HttpClient _http = new()
    {
        BaseAddress = new Uri("http://localhost:5023/api/")
    };

    public async Task<ApiResponse<LoginData>> LoginAsync(LoginRequest request)
    {
        try
        {
            var response = await _http.PostAsJsonAsync("auth/login", request);
            var resultado = await response.Content
                .ReadFromJsonAsync<ApiResponse<LoginData>>();

            return resultado ?? new ApiResponse<LoginData>
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