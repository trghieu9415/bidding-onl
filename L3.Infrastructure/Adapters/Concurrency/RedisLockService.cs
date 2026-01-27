using L2.Application.Ports.Concurrency;
using RedLockNet;

namespace L3.Infrastructure.Adapters.Concurrency;

public class RedisLockService(IDistributedLockFactory lockFactory) : IDistributedLockService {
  public async Task<IDisposable?> AcquireLockAsync(string resourceKey, TimeSpan expiry, TimeSpan wait) {
    var redLock = await lockFactory.CreateLockAsync(resourceKey, expiry, wait, TimeSpan.FromMilliseconds(200));
    return redLock.IsAcquired ? redLock : null;
  }
}
