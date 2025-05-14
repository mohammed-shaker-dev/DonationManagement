using System.Linq;
namespace Dashboard.Api
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
        public async Task Invoke(HttpContext context)
        {
            // Skip API key validation for static files and uploads
            if (IsExcludedPath(context.Request.Path))
            {
                await _next(context);
                return;
            }

            if (!context.Request.Headers.TryGetValue("x-api-key", out var extractedApiKey))
            {
                context.Response.StatusCode = 401; // Unauthorized
                await context.Response.WriteAsync("API Key is missing");
                return;
            }
            var allowedKeys = _configuration.GetSection("AllowedApiKeys").Get<Dictionary<string, string>>();
            if (!allowedKeys.Values.Contains(extractedApiKey.ToString()))
            {
                context.Response.StatusCode = 403; // Forbidden
                await context.Response.WriteAsync("Invalid API Key");
                return;
            }
            await _next(context);
        }

        private bool IsExcludedPath(PathString path)
        {
            // Define paths that don't require API key
            string[] excludedPaths = new[]
            {
                "/uploads",           // All files in uploads
                "/favicon.ico",       // Favicon
                "/images",            // Static images
                "/css",               // CSS files
                "/js",                // JavaScript files
                "/lib"                // Library files
            };

            return excludedPaths.Any(excludedPath =>
                path.StartsWithSegments(excludedPath, StringComparison.OrdinalIgnoreCase));
        }
    }
}