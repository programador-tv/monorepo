namespace Domain.Redis;

public interface IRedisContext
{
    Task<string?> GetValueAsync(string key);
    Task SetValueAsync(string key, string value);
    Task SaveMessageAsync<T>(string key, T message);
}
