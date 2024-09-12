using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace SavingsBook.Application.Redis;

public class RedisCacheService
{
    private readonly IDistributedCache _cache;

    public RedisCacheService(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task RemoveCache(string key)
    {
        _cache.Remove(key);
    }

    // public async Task<List<StoreCategory>> GetCacheStoreCategory()
    // {
    //     var result = new List<StoreCategory>();
    //     var cached = await _cache.GetStringAsync(FoodStoreConsts.Cache.StoreCategoryKey);
    //     if (cached != null)
    //     {
    //         result = JsonConvert.DeserializeObject<List<StoreCategory>>(cached);
    //     }
    //     else
    //     {
    //         result = (await _storeCategoryRepository.GetQueryableAsync()).ToList();
    //
    //         await _cache.SetStringAsync(FoodStoreConsts.Cache.StoreCategoryKey, JsonConvert.SerializeObject(result),
    //             new DistributedCacheEntryOptions() { AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(30) });
    //     }
    //     return result;
    // }

}