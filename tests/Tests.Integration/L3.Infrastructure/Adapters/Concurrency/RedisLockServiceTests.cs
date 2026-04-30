using L3.Infrastructure.Adapters.Concurrency;
using Medallion.Threading;
using Tests.Integration.TestSupport;
using Xunit;

namespace Tests.Integration.L3.Infrastructure.Adapters.Concurrency;

public class RedisLockServiceTests {
  [Fact]
  public async Task AcquireLockAsync_returns_handle_when_provider_succeeds() {
    var expectedHandle = DynamicProxyFactory.Create<IDistributedSynchronizationHandle>((method, _) => method.Name switch {
      "Dispose" => null,
      "DisposeAsync" => AsyncReturn.For(method),
      _ => null
    });
    var provider = DynamicProxyFactory.Create<IDistributedLockProvider>((method, _) => method.Name switch {
      "TryAcquireLockAsync" => AsyncReturn.For(method, expectedHandle),
      _ => throw new NotSupportedException()
    });

    var service = new RedisLockService(provider);
    var handle = await service.AcquireLockAsync("resource", TimeSpan.FromSeconds(1));

    Assert.Same(expectedHandle, handle);
  }

  [Fact]
  public async Task AcquireLockAsync_returns_null_when_provider_throws() {
    var provider = DynamicProxyFactory.Create<IDistributedLockProvider>((method, _) => method.Name switch {
      "TryAcquireLockAsync" => throw new InvalidOperationException("redis unavailable"),
      _ => throw new NotSupportedException()
    });

    var service = new RedisLockService(provider);
    var handle = await service.AcquireLockAsync("resource", TimeSpan.FromSeconds(1));

    Assert.Null(handle);
  }
}
