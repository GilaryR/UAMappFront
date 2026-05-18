using SistemaHorario.UI.State;
using SistemaHorarios.Application.Common;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace SistemaHorario.Infrastructure.Api;

public class ApiClient
{
    private static readonly HttpClient _http = new()
    {
        BaseAddress = new Uri("http://localhost:5023/api/")
    };

    private void AgregarToken()
    {
        if (!string.IsNullOrWhiteSpace(UsuarioSesion.Token))
            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", UsuarioSesion.Token);
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
            var data = await response.Content.ReadFromJsonAsync<T>();
            return new ApiResponse<T> { Success = response.IsSuccessStatusCode, Data = data };
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