namespace SistemaHorarios.Application.Common;

public static class ApiConfiguration
{
    private const string DefaultBaseUrl = "http://localhost:5023/api/";
    private const string BaseUrlEnvironmentVariable = "SISTEMA_HORARIOS_API_BASE_URL";

    public static Uri BaseUri
    {
        get
        {
            string? configuredUrl =
                Environment.GetEnvironmentVariable(BaseUrlEnvironmentVariable);

            string baseUrl = string.IsNullOrWhiteSpace(configuredUrl)
                ? DefaultBaseUrl
                : configuredUrl.Trim();

            if (!baseUrl.EndsWith('/'))
                baseUrl += "/";

            return new Uri(baseUrl);
        }
    }
}
