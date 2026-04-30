using System.Linq.Expressions;
using L2.Application.DTOs;
using L2.Application.Ports.Cache;
using L2.Application.Repositories.Read;
using L3.Infrastructure.Adapters.Cache;
using L3.Infrastructure.Services.Abstractions;
using Medallion.Threading;
using Tests.Integration.TestSupport;
using Xunit;

namespace Tests.Integration.L3.Infrastructure.Adapters.Cache;

public class BusinessCacheTests {
  [Fact]
  public async Task GetCurrentSessionsAsync_returns_cached_value_without_hitting_repository() {
    var cached = new List<AuctionSessionDto> {
      new() { Title = "cached-session" }
    };
    var cache = new FakeCacheService { ValueByKey = { [BusinessKeys.CurrentSession] = cached } };
    var repository = new FakeSessionReadRepository();
    var service = new BusinessCache(cache, repository, CreateLockProvider(acquireHandle: true));

    var result = await service.GetCurrentSessionsAsync(CancellationToken.None);

    Assert.Equal("cached-session", result[0].Title);
    Assert.Equal(0, repository.FetchCalls);
  }

  [Fact]
  public async Task GetCurrentSessionsAsync_fetches_and_caches_when_cache_is_empty() {
    var repositoryData = new List<AuctionSessionDto> {
      new() { Title = "repo-session" }
    };
    var cache = new FakeCacheService();
    var repository = new FakeSessionReadRepository { CurrentSessions = repositoryData };
    var service = new BusinessCache(cache, repository, CreateLockProvider(acquireHandle: true));

    var result = await service.GetCurrentSessionsAsync(CancellationToken.None);

    Assert.Equal("repo-session", result[0].Title);
    Assert.Equal(1, repository.FetchCalls);
    Assert.Same(repositoryData, cache.ValueByKey[BusinessKeys.CurrentSession]);
    Assert.NotNull(cache.LastExpiration);
    Assert.InRange(cache.LastExpiration!.Value, TimeSpan.FromDays(7), TimeSpan.FromDays(7).Add(TimeSpan.FromSeconds(60)));
  }

  [Fact]
  public async Task GetCurrentSessionsAsync_throws_when_distributed_lock_cannot_be_acquired() {
    var service = new BusinessCache(
      new FakeCacheService(),
      new FakeSessionReadRepository(),
      CreateLockProvider(acquireHandle: false)
    );

    await Assert.ThrowsAsync<TimeoutException>(() => service.GetCurrentSessionsAsync(CancellationToken.None));
  }

  private static IDistributedLockProvider CreateLockProvider(bool acquireHandle) {
    var handle = DynamicProxyFactory.Create<IDistributedSynchronizationHandle>((method, _) => method.Name switch {
      "Dispose" => null,
      "DisposeAsync" => AsyncReturn.For(method),
      "HandleLostToken" => null,
      _ => null
    });

    return DynamicProxyFactory.Create<IDistributedLockProvider>((method, _) => method.Name switch {
      "TryAcquireLockAsync" => AsyncReturn.For(method, acquireHandle ? handle : null),
      _ => throw new NotSupportedException($"IDistributedLockProvider.{method.Name} is not configured for this test.")
    });
  }

  private sealed class FakeCacheService : ICacheService {
    public Dictionary<string, object?> ValueByKey { get; } = new();
    public TimeSpan? LastExpiration { get; private set; }

    public Task<T?> GetAsync<T>(string key, CancellationToken ct = default) {
      return Task.FromResult(ValueByKey.TryGetValue(key, out var value) ? (T?)value : default);
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken ct = default) {
      ValueByKey[key] = value;
      LastExpiration = expiration;
      return Task.CompletedTask;
    }

    public Task RemoveAsync(string key, CancellationToken ct = default) {
      ValueByKey.Remove(key);
      return Task.CompletedTask;
    }
  }

  private sealed class FakeSessionReadRepository : ISessionReadRepository {
    public int FetchCalls { get; private set; }
    public List<AuctionSessionDto> CurrentSessions { get; init; } = [];

    public Task<List<AuctionSessionDto>> GetCurrentSessionsAsync(CancellationToken ct = default) {
      FetchCalls++;
      return Task.FromResult(CurrentSessions);
    }

    public Task<AuctionSessionDto?> GetByIdAsync(Guid id, CancellationToken ct = default) {
      throw new NotSupportedException();
    }

    public Task<(int total, List<AuctionSessionDto> entities)> GetAsync(
      Expression<Func<L1.Core.Domain.Bidding.Entities.AuctionSession, bool>>? criteria = null,
      AutoFilterer.Abstractions.IFilter? filter = null,
      List<Expression<Func<L1.Core.Domain.Bidding.Entities.AuctionSession, object>>>? includes = null,
      CancellationToken ct = default
    ) {
      throw new NotSupportedException();
    }

    public Task<(int total, List<AuctionSessionDto> entities)> GetDeletedAsync(
      Expression<Func<L1.Core.Domain.Bidding.Entities.AuctionSession, bool>>? criteria = null,
      AutoFilterer.Abstractions.IFilter? filter = null,
      List<Expression<Func<L1.Core.Domain.Bidding.Entities.AuctionSession, object>>>? includes = null,
      CancellationToken ct = default
    ) {
      throw new NotSupportedException();
    }

    public Task<AuctionSessionDto?> GetFirstAsync(
      Expression<Func<L1.Core.Domain.Bidding.Entities.AuctionSession, bool>>? criteria = null,
      CancellationToken ct = default
    ) {
      throw new NotSupportedException();
    }
  }
}
