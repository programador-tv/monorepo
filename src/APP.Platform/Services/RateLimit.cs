using System;

namespace Platform.Services;

public class RateLimit
{
    private readonly Dictionary<string, DateTime> _lastRequestTimestamps = new();
    private readonly TimeSpan _rateTimeSpan = TimeSpan.FromSeconds(1);

    public bool IsRateLimited(string userId, string liveId)
    {
        var key = $"{userId}-{liveId}";
        var result = false;
        if (!_lastRequestTimestamps.TryGetValue(key, out var value))
        {
            value = DateTime.UtcNow;
            _lastRequestTimestamps[key] = value;
            return result;
        }

        if (DateTime.UtcNow - value < _rateTimeSpan)
        {
            result = true;
        }
        else
        {
            _lastRequestTimestamps[key] = DateTime.Now;
        }

        return result;
    }
}
