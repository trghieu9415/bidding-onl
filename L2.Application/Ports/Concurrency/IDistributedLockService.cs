namespace L2.Application.Ports.Concurrency;

public interface IDistributedLockService {
  Task<IDisposable?> AcquireLockAsync(string resourceKey, TimeSpan wait);
}
