using L3.Infrastructure.Services;
using Tests.Integration.TestSupport;
using Xunit;

namespace Tests.Integration.L3.Infrastructure.Services;

public class RedisCacheServiceTests {
  [Fact]
  public async Task GetAsync_returns_default_when_key_is_missing() {
    var cache = new InMemoryDistributedCache();
    var service = new RedisCacheService(cache);

    var result = await service.GetAsync<CacheItem>("missing");

    Assert.Null(result);
  }

  [Fact]
  public async Task SetAsync_and_GetAsync_round_trip_serialized_value() {
    var cache = new InMemoryDistributedCache();
    var service = new RedisCacheService(cache);
    var item = new CacheItem("auction-1", 120000);

    await service.SetAsync("current", item, TimeSpan.FromMinutes(5));
    var result = await service.GetAsync<CacheItem>("current");

    Assert.NotNull(result);
    Assert.Equal(item.Name, result!.Name);
    Assert.Equal(item.Price, result.Price);
    Assert.Equal(TimeSpan.FromMinutes(5), cache.OptionsByKey["current"].AbsoluteExpirationRelativeToNow);
  }

  [Fact]
  public async Task SetAsync_uses_default_expiration_when_none_is_provided() {
    var cache = new InMemoryDistributedCache();
    var service = new RedisCacheService(cache);

    await service.SetAsync("default-exp", new CacheItem("auction-1", 5));

    Assert.Equal(TimeSpan.FromMinutes(60), cache.OptionsByKey["default-exp"].AbsoluteExpirationRelativeToNow);
  }

  [Fact]
  public async Task RemoveAsync_deletes_existing_key() {
    var cache = new InMemoryDistributedCache();
    var service = new RedisCacheService(cache);
    await service.SetAsync("remove-me", new CacheItem("auction-1", 5));

    await service.RemoveAsync("remove-me");
    var result = await service.GetAsync<CacheItem>("remove-me");

    Assert.Null(result);
  }

  private sealed record CacheItem(string Name, decimal Price);
}
