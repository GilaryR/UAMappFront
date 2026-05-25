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


    public async Task<ApiResponse<T>> PutFileAsync<T>(
    string url,
    string nombreCampo,
    string rutaArchivo)
{
    AgregarToken();

    try
    {
        using MultipartFormDataContent form = new();

        await using FileStream stream =
            File.OpenRead(rutaArchivo);

        StreamContent fileContent = new(stream);

        fileContent.Headers.ContentType =
            new MediaTypeHeaderValue(
                ObtenerContentType(rutaArchivo)
            );

        form.Add(
            fileContent,
            nombreCampo,
            Path.GetFileName(rutaArchivo)
        );

        HttpResponseMessage response =
            await _http.PutAsync(url, form);

        if (!response.IsSuccessStatusCode)
        {
            string mensaje =
                await response.Content.ReadAsStringAsync();

            return new ApiResponse<T>
            {
                Success = false,
                Message = mensaje
            };
        }

        ApiResponse<T>? resultado =
            await response.Content.ReadFromJsonAsync<ApiResponse<T>>();

        if (resultado == null)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = "No se pudo leer la respuesta del servidor."
            };
        }

        return resultado;
    }
    catch (Exception ex)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = ex.Message
        };
    }
}

private static string ObtenerContentType(string rutaArchivo)
{
    string extension =
        Path.GetExtension(rutaArchivo).ToLower();

    return extension switch
    {
        ".jpg" => "image/jpeg",
        ".jpeg" => "image/jpeg",
        ".png" => "image/png",
        ".bmp" => "image/bmp",
        _ => "application/octet-stream"
    };
}

    private void AgregarToken()
    {
        if (!string.IsNullOrWhiteSpace(Token))
        {
            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", Token);
        }
        else
        {
            _http.DefaultRequestHeaders.Authorization = null;
        }
    }

    private static async Task<ApiResponse<T>> ProcesarRespuestaAsync<T>(
        HttpResponseMessage response)
    {
        try
        {
            ApiResponse<T>? apiResponse =
                await response.Content.ReadFromJsonAsync<ApiResponse<T>>();

            if (apiResponse != null)
            {
                return apiResponse;
            }

            return new ApiResponse<T>
            {
                Success = false,
                Message = "La API no devolvió una respuesta válida."
            };
        }
        catch (Exception ex)
        {
            string contenido = await response.Content.ReadAsStringAsync();

            return new ApiResponse<T>
            {
                Success = false,
                Message = string.IsNullOrWhiteSpace(contenido)
                    ? ex.Message
                    : contenido
            };
        }
    }

    public async Task<ApiResponse<T>> GetAsync<T>(string url)
    {
        AgregarToken();

        try
        {
            HttpResponseMessage response = await _http.GetAsync(url);

            string contenido = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return new ApiResponse<T>
                {
                    Success = false,
                    Message = string.IsNullOrWhiteSpace(contenido)
                        ? "Error al consultar la información."
                        : contenido
                };
            }

            if (string.IsNullOrWhiteSpace(contenido))
            {
                return new ApiResponse<T>
                {
                    Success = false,
                    Message = "La respuesta del servidor está vacía."
                };
            }

            try
            {
                ApiResponse<T>? apiResponse =
                    JsonSerializer.Deserialize<ApiResponse<T>>(
                        contenido,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });

                if (apiResponse != null &&
                    (apiResponse.Success ||
                     apiResponse.Data != null ||
                     !string.IsNullOrWhiteSpace(apiResponse.Message)))
                {
                    return apiResponse;
                }
            }
            catch
            {
                // Si no viene como ApiResponse<T>, se intenta leer como respuesta directa.
            }

            T? data = JsonSerializer.Deserialize<T>(
                contenido,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            return new ApiResponse<T>
            {
                Success = true,
                Message = "Consulta realizada correctamente.",
                Data = data
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = ex.Message
            };
        }
    }

    public async Task<ApiResponse<string>> PostAsync(string url, object body)
    {
        AgregarToken();

        try
        {
            HttpResponseMessage response = await _http.PostAsJsonAsync(url, body);

            ApiResponse<object> apiResponse =
                await ProcesarRespuestaAsync<object>(response);

            return new ApiResponse<string>
            {
                Success = apiResponse.Success,
                Message = apiResponse.Message,
                Data = apiResponse.Data?.ToString()
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<string>
            {
                Success = false,
                Message = ex.Message
            };
        }
    }


    public async Task<ApiResponse<T>> PatchAsync<T>(string url, object data)
    {
        AgregarToken();

        try
        {
            HttpResponseMessage response =
                await _http.PatchAsJsonAsync(url, data);

            if (!response.IsSuccessStatusCode)
            {
                string mensaje =
                    await response.Content.ReadAsStringAsync();

                return new ApiResponse<T>
                {
                    Success = false,
                    Message = mensaje
                };
            }

            ApiResponse<T>? resultado =
                await response.Content.ReadFromJsonAsync<ApiResponse<T>>();

            return resultado ?? new ApiResponse<T>
            {
                Success = false,
                Message = "No se pudo leer la respuesta del servidor."
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = ex.Message
            };
        }
    }
    public async Task<ApiResponse<T>> PostAsync<T>(string url, object body)
    {
        AgregarToken();

        try
        {
            HttpResponseMessage response = await _http.PostAsJsonAsync(url, body);

            return await ProcesarRespuestaAsync<T>(response);
        }
        catch (Exception ex)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = ex.Message
            };
        }
    }

    public async Task<ApiResponse<string>> PutAsync(string url, object body)
    {
        AgregarToken();

        try
        {
            HttpResponseMessage response = await _http.PutAsJsonAsync(url, body);

            ApiResponse<object> apiResponse =
                await ProcesarRespuestaAsync<object>(response);

            return new ApiResponse<string>
            {
                Success = apiResponse.Success,
                Message = apiResponse.Message,
                Data = apiResponse.Data?.ToString()
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<string>
            {
                Success = false,
                Message = ex.Message
            };
        }
    }

    public async Task<ApiResponse<T>> PutAsync<T>(
    string url,
    object data)
{
    AgregarToken();

    try
    {
        HttpResponseMessage response =
            await _http.PutAsJsonAsync(url, data);

        if (!response.IsSuccessStatusCode)
        {
            string mensaje =
                await response.Content.ReadAsStringAsync();

            return new ApiResponse<T>
            {
                Success = false,
                Message = mensaje
            };
        }

        ApiResponse<T>? resultado =
            await response.Content.ReadFromJsonAsync<ApiResponse<T>>();

        return resultado ?? new ApiResponse<T>
        {
            Success = false,
            Message = "No se pudo leer la respuesta del servidor."
        };
    }
    catch (Exception ex)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = ex.Message
        };
    }
}
    

    public async Task<ApiResponse<T>> DeleteAsync<T>(string url)
    {
        AgregarToken();

        try
        {
            HttpResponseMessage response =
                await _http.DeleteAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                string mensaje =
                    await response.Content.ReadAsStringAsync();

                return new ApiResponse<T>
                {
                    Success = false,
                    Message = mensaje
                };
            }

            ApiResponse<T>? resultado =
                await response.Content.ReadFromJsonAsync<ApiResponse<T>>();

            return resultado ?? new ApiResponse<T>
            {
                Success = false,
                Message = "No se pudo leer la respuesta del servidor."
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = ex.Message
            };
        }
    }
}