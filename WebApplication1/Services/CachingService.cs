using Microsoft.Extensions.Caching.Memory;

namespace WebApplication1.Services
{
    public interface ICachingService
    {
        Task<T?> GetAsync<T>(string key) where T : class;
        Task SetAsync<T>(string key, T value, TimeSpan? expiration = null) where T : class;
        Task RemoveAsync(string key);
        Task RemoveByPatternAsync(string pattern);
    }

    public class MemoryCachingService : ICachingService
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger<MemoryCachingService> _logger;
        private readonly HashSet<string> _cacheKeys = new();
        private readonly object _lock = new();

        public MemoryCachingService(IMemoryCache cache, ILogger<MemoryCachingService> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public Task<T?> GetAsync<T>(string key) where T : class
        {
            try
            {
                var value = _cache.Get<T>(key);
                if (value != null)
                {
                    _logger.LogDebug("Cache hit for key: {Key}", key);
                }
                return Task.FromResult(value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving value from cache for key: {Key}", key);
                return Task.FromResult<T?>(null);
            }
        }

        public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null) where T : class
        {
            try
            {
                var options = new MemoryCacheEntryOptions();
                
                if (expiration.HasValue)
                {
                    options.AbsoluteExpirationRelativeToNow = expiration;
                }
                else
                {
                    options.SlidingExpiration = TimeSpan.FromMinutes(30); // Default 30 minutes
                }

                options.RegisterPostEvictionCallback((key, value, reason, state) =>
                {
                    lock (_lock)
                    {
                        _cacheKeys.Remove(key.ToString()!);
                    }
                });

                _cache.Set(key, value, options);
                
                lock (_lock)
                {
                    _cacheKeys.Add(key);
                }

                _logger.LogDebug("Cached value for key: {Key}", key);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting cache value for key: {Key}", key);
                return Task.CompletedTask;
            }
        }

        public Task RemoveAsync(string key)
        {
            try
            {
                _cache.Remove(key);
                lock (_lock)
                {
                    _cacheKeys.Remove(key);
                }
                _logger.LogDebug("Removed cache entry for key: {Key}", key);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing cache entry for key: {Key}", key);
                return Task.CompletedTask;
            }
        }

        public Task RemoveByPatternAsync(string pattern)
        {
            try
            {
                var keysToRemove = new List<string>();
                
                lock (_lock)
                {
                    keysToRemove.AddRange(_cacheKeys.Where(key => key.Contains(pattern)));
                }

                foreach (var key in keysToRemove)
                {
                    _cache.Remove(key);
                    lock (_lock)
                    {
                        _cacheKeys.Remove(key);
                    }
                }

                _logger.LogDebug("Removed {Count} cache entries matching pattern: {Pattern}", keysToRemove.Count, pattern);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing cache entries by pattern: {Pattern}", pattern);
                return Task.CompletedTask;
            }
        }
    }
}
