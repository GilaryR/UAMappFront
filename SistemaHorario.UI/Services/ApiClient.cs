using SistemaHorarios.Application.Common;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace SistemaHorario.UI.Services;

public class ApiClient
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private static readonly HttpClient _http = new()
    {
        BaseAddress = ApiConfiguration.BaseUri
    };

    public static string? Token { get; set; }

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
            HttpResponseMessage response = await _http.GetAsync(url);
            return await LeerRespuestaAsync<T>(response);
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
            HttpResponseMessage response = await _http.PostAsJsonAsync(url, body);
            return await LeerRespuestaTextoAsync(response);
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
            HttpResponseMessage response = await _http.PostAsJsonAsync(url, body);
            return await LeerRespuestaAsync<T>(response);
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
            HttpResponseMessage response = await _http.PutAsJsonAsync(url, body);
            return await LeerRespuestaTextoAsync(response);
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
            HttpResponseMessage response = await _http.DeleteAsync(url);
            return await LeerRespuestaTextoAsync(response);
        }
        catch (Exception ex)
        {
            return new ApiResponse<string> { Success = false, Message = ex.Message };
        }
    }

    private static async Task<ApiResponse<T>> LeerRespuestaAsync<T>(
        HttpResponseMessage response)
    {
        string contenido = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = ExtraerMensaje(contenido, response.ReasonPhrase)
            };
        }

        if (string.IsNullOrWhiteSpace(contenido))
            return new ApiResponse<T> { Success = true };

        if (typeof(T) == typeof(string))
        {
            object texto = ExtraerMensaje(contenido, contenido);
            return new ApiResponse<T> { Success = true, Data = (T)texto };
        }

        try
        {
            if (TryLeerApiResponse(contenido, out ApiResponse<T> respuestaEnvoltorio))
                return respuestaEnvoltorio;

            T? data = JsonSerializer.Deserialize<T>(contenido, JsonOptions);

            return new ApiResponse<T>
            {
                Success = true,
                Data = data
            };
        }
        catch (JsonException ex)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = $"No se pudo interpretar la respuesta de la API: {ex.Message}"
            };
        }
    }

    private static async Task<ApiResponse<string>> LeerRespuestaTextoAsync(
        HttpResponseMessage response)
    {
        string contenido = await response.Content.ReadAsStringAsync();
        string mensaje = ExtraerMensaje(contenido, response.ReasonPhrase);

        return new ApiResponse<string>
        {
            Success = response.IsSuccessStatusCode,
            Message = mensaje,
            Data = response.IsSuccessStatusCode ? contenido : null
        };
    }

    private static bool PareceApiResponse(string contenido)
    {
        try
        {
            using JsonDocument document = JsonDocument.Parse(contenido);

            if (document.RootElement.ValueKind != JsonValueKind.Object)
                return false;

            return TienePropiedad(document.RootElement, "success")
                || TienePropiedad(document.RootElement, "message")
                || TienePropiedad(document.RootElement, "data");
        }
        catch (JsonException)
        {
            return false;
        }
    }

    private static bool TryLeerApiResponse<T>(
        string contenido,
        out ApiResponse<T> response)
    {
        response = new ApiResponse<T>();

        using JsonDocument document = JsonDocument.Parse(contenido);

        if (document.RootElement.ValueKind != JsonValueKind.Object)
            return false;

        JsonElement root = document.RootElement;

        if (!PareceApiResponse(contenido))
            return false;

        bool success = true;
        string message = string.Empty;
        T? data = default;

        if (TryGetProperty(root, "success", out JsonElement successElement)
            && successElement.ValueKind is JsonValueKind.True or JsonValueKind.False)
        {
            success = successElement.GetBoolean();
        }

        if (TryGetProperty(root, "message", out JsonElement messageElement))
            message = LeerJsonComoTexto(messageElement);

        if (TryGetProperty(root, "data", out JsonElement dataElement)
            && dataElement.ValueKind != JsonValueKind.Null
            && dataElement.ValueKind != JsonValueKind.Undefined)
        {
            data = JsonSerializer.Deserialize<T>(dataElement.GetRawText(), JsonOptions);
        }

        response = new ApiResponse<T>
        {
            Success = success,
            Message = message,
            Data = data
        };

        return true;
    }

    private static string ExtraerMensaje(string contenido, string? fallback)
    {
        if (string.IsNullOrWhiteSpace(contenido))
            return fallback ?? string.Empty;

        try
        {
            using JsonDocument document = JsonDocument.Parse(contenido);
            JsonElement root = document.RootElement;

            if (root.ValueKind == JsonValueKind.String)
                return root.GetString() ?? string.Empty;

            if (root.ValueKind == JsonValueKind.Array)
                return string.Join(Environment.NewLine, root.EnumerateArray().Select(LeerJsonComoTexto));

            if (root.ValueKind == JsonValueKind.Object)
            {
                if (TryGetProperty(root, "message", out JsonElement message))
                    return LeerJsonComoTexto(message);

                if (TryGetProperty(root, "mensaje", out JsonElement mensaje))
                    return LeerJsonComoTexto(mensaje);

                if (TryGetProperty(root, "errores", out JsonElement errores))
                    return LeerJsonComoTexto(errores);

                if (TryGetProperty(root, "errors", out JsonElement errors))
                    return LeerJsonComoTexto(errors);
            }
        }
        catch (JsonException)
        {
            return contenido;
        }

        return string.IsNullOrWhiteSpace(fallback) ? contenido : fallback;
    }

    private static bool TryGetProperty(
        JsonElement element,
        string propertyName,
        out JsonElement value)
    {
        foreach (JsonProperty property in element.EnumerateObject())
        {
            if (string.Equals(property.Name, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                value = property.Value;
                return true;
            }
        }

        value = default;
        return false;
    }

    private static bool TienePropiedad(JsonElement element, string propertyName)
    {
        return TryGetProperty(element, propertyName, out _);
    }

    private static string LeerJsonComoTexto(JsonElement element)
    {
        return element.ValueKind switch
        {
            JsonValueKind.String => element.GetString() ?? string.Empty,
            JsonValueKind.Array => string.Join(Environment.NewLine, element.EnumerateArray().Select(LeerJsonComoTexto)),
            _ => element.ToString()
        };
    }
}
