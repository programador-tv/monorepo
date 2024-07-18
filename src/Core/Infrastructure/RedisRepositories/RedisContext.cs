// using System.Text.Json;
// using Domain.Redis;
// using StackExchange.Redis;

// namespace Infrastructure.Redis;

// public sealed class RedisContext(IConnectionMultiplexer _redis) : IRedisContext
// {
//     public async Task<string?> GetValueAsync(string key)
//     {
//         var db = _redis.GetDatabase();
//         return await db.StringGetAsync(key);
//     }

//     public async Task SetValueAsync(string key, string value)
//     {
//         var db = _redis.GetDatabase();
//         await db.StringSetAsync(key, value);
//     }

//     public async Task SaveMessageAsync<T>(string key, T message)
//     {
//         var db = _redis.GetDatabase();
//         var jsonMessage = JsonSerializer.Serialize(message);
//         await db.ListLeftPushAsync(key, jsonMessage);
//     }
// }
