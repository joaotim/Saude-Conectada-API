using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

public class ApiKeyMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string _apiKeyConfigurada = "123";
    private const string APIKEY_HEADER_NAME = "X-API-KEY";

    public ApiKeyMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _apiKeyConfigurada = configuration.GetValue<string>("ApiKeySettings:ApiKey");
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Headers.TryGetValue(APIKEY_HEADER_NAME, out var extractedApiKey))
        {
            context.Response.StatusCode = 401; 
            await context.Response.WriteAsync($"Chave de API não fornecida. Valor da API {extractedApiKey} ");
            return;
        }

        if (!string.Equals(_apiKeyConfigurada, extractedApiKey))
        {
            context.Response.StatusCode = 403; 
            await context.Response.WriteAsync($"Chave de API inválida. Valor API {_apiKeyConfigurada}, Valor Extrat {extractedApiKey}");
            return;
        }

        await _next(context);
    }
}
