using L2.Application.Ports.Concurrency;
using Medallion.Threading;

namespace L3.Infrastructure.Adapters.Concurrency;

public class RedisLockService(IDistributedLockProvider lockProvider) : IDistributedLockService {
  public async Task<IDisposable?> AcquireLockAsync(string resourceKey, TimeSpan wait) {
    try {
      var handle = await lockProvider.TryAcquireLockAsync(resourceKey, wait);
      return handle;
    } catch {
      return null;
    }
  }
}
