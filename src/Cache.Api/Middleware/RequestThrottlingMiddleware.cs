using Microsoft.Extensions.Caching.Memory;

public class RequestThrottlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestThrottlingMiddleware> _logger;
    private readonly IMemoryCache _cache;
    private readonly int _limit = 100;
    private readonly TimeSpan _window = TimeSpan.FromSeconds(1);

    public RequestThrottlingMiddleware(RequestDelegate next, ILogger<RequestThrottlingMiddleware> logger, IMemoryCache cache)
    {
        _next = next;
        _logger = logger;
        _cache = cache;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var key = $"throttle:{ip}";
        var count = _cache.Get<int?>(key) ?? 0;
        if (count >= _limit)
        {
            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            await context.Response.WriteAsync("Too many requests");
            return;
        }
        _cache.Set(key, count + 1, _window);
        await _next(context);
    }
}
