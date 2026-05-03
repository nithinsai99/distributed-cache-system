using System.Text.Json;
using StackExchange.Redis;
using Cache.Core.Interfaces;

namespace Cache.Infrastructure.Services
{
    public class RedisCacheService : ICacheService, IDisposable
    {
        private readonly ConnectionMultiplexer _connection;
        private readonly IDatabase _db;

        public RedisCacheService(string configuration)
        {
            _connection = ConnectionMultiplexer.Connect(configuration);
            _db = _connection.GetDatabase();
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            var value = await _db.StringGetAsync(key).ConfigureAwait(false);
            if (value.IsNullOrEmpty) return default;
            var str = value.ToString();
            if (string.IsNullOrEmpty(str)) return default;
            return JsonSerializer.Deserialize<T>(str);
        }

        public Task SetAsync<T>(string key, T value, TimeSpan? ttl = null)
        {
            var json = JsonSerializer.Serialize(value);
            return _db.StringSetAsync(key, json, ttl);
        }

        public Task RemoveAsync(string key) => _db.KeyDeleteAsync(key);

        public void Dispose() => _connection?.Dispose();
    }
}
