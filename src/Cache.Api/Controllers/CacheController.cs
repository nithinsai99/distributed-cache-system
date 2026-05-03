using Cache.Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class CacheController : ControllerBase
{
    private readonly ICacheService _cache;
    public CacheController(ICacheService cache) => _cache = cache;

    [HttpGet("{key}")]
    public async Task<IActionResult> Get(string key)
    {
        var value = await _cache.GetAsync<string>(key);
        if (value is null) return NotFound();
        return Ok(value);
    }

    [HttpPost("{key}")]
    public async Task<IActionResult> Set(string key, [FromBody] object body)
    {
        await _cache.SetAsync<object>(key, body);
        return NoContent();
    }

    [HttpDelete("{key}")]
    public async Task<IActionResult> Remove(string key)
    {
        await _cache.RemoveAsync(key);
        return NoContent();
    }
}
