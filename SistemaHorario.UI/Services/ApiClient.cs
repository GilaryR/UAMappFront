using SistemaHorarios.Application.Common;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace SistemaHorario.UI.Services;

public class ApiClient
{
    private const string BaseUrlPorDefecto = "http://localhost:5023/api/";

    private static readonly HttpClient _http = new()
    {
        BaseAddress = new Uri(ObtenerBaseUrl())
    };

    public static string? Token { get; set; }

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

    private void AgregarToken()
    {
        if (!string.IsNullOrWhiteSpace(Token))
            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", Token);
        else
            _http.DefaultRequestHeaders.Authorization = null;
    }

    public async Task<ApiResponse<T>> GetAsync<T>(string url)
    {
        AgregarToken();
        try
        {
            var data = await _http.GetFromJsonAsync<T>(url);
            return new ApiResponse<T> { Success = true, Data = data };
        }
        catch (Exception ex)
        {
            return new ApiResponse<T> { Success = false, Message = ex.Message };
        }
    }

    public async Task<ApiResponse<string>> PostAsync(string url, object body)
    {
        AgregarToken();
        try
        {
            var response = await _http.PostAsJsonAsync(url, body);
            var mensaje = await response.Content.ReadAsStringAsync();
            return new ApiResponse<string> { Success = response.IsSuccessStatusCode, Message = mensaje };
        }
        catch (Exception ex)
        {
            return new ApiResponse<string> { Success = false, Message = ex.Message };
        }
    }

    public async Task<ApiResponse<T>> PostAsync<T>(string url, object body)
    {
        AgregarToken();
        try
        {
            var response = await _http.PostAsJsonAsync(url, body);
            if (!response.IsSuccessStatusCode)
            {
                var msg = await response.Content.ReadAsStringAsync();
                return new ApiResponse<T> { Success = false, Message = msg };
            }
            var data = await response.Content.ReadFromJsonAsync<T>();
            return new ApiResponse<T> { Success = true, Data = data };
        }
        catch (Exception ex)
        {
            return new ApiResponse<T> { Success = false, Message = ex.Message };
        }
    }

    public async Task<ApiResponse<string>> PutAsync(string url, object body)
    {
        AgregarToken();
        try
        {
            var response = await _http.PutAsJsonAsync(url, body);
            var mensaje = await response.Content.ReadAsStringAsync();
            return new ApiResponse<string> { Success = response.IsSuccessStatusCode, Message = mensaje };
        }
        catch (Exception ex)
        {
            return new ApiResponse<string> { Success = false, Message = ex.Message };
        }
    }

    public async Task<ApiResponse<string>> DeleteAsync(string url)
    {
        AgregarToken();
        try
        {
            var response = await _http.DeleteAsync(url);
            var mensaje = await response.Content.ReadAsStringAsync();
            return new ApiResponse<string> { Success = response.IsSuccessStatusCode, Message = mensaje };
        }
        catch (Exception ex)
        {
            return new ApiResponse<string> { Success = false, Message = ex.Message };
        }
    }
}
