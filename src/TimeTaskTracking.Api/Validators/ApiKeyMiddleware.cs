using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using TimeTaskTracking.Shared.CommonResult;
using TimeTaskTracking.Shared.Resources;

namespace TimeTaskTracking.Validators
{
    public class ApiKeyMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;
        public ApiKeyMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var apiKeys = _configuration.GetSection("ApiKeys").GetChildren();
            if (!context.Request.Headers.TryGetValue("X-API-KEY", out var extractedApiKey))
            {
                context.Response.StatusCode = 403;
                await context.Response.WriteAsync(SharedResources.ApiKeyNotProvided);
                return;
            }
            if (!IsApiKeyValid(apiKeys, extractedApiKey))
            {
                context.Response.StatusCode = 403;
                await context.Response.WriteAsync(SharedResources.UnauthorizedClient);
                return;
            }

            await _next(context);
        }

        private bool IsApiKeyValid(IEnumerable<IConfigurationSection> apiKeys, string extractedApiKey)
        {
            var apiKeyEntry = apiKeys.FirstOrDefault(key =>
            key["Key"] == extractedApiKey);

            if (apiKeyEntry != null)
            {
                var createdAt = DateTime.Parse(apiKeyEntry["CreatedAt"]);
                return createdAt.AddDays(30) >= DateTime.UtcNow;
            }

            return false;
        }
    }
}
