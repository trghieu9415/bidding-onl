namespace L2.Application.Abstractions;

public interface ILockable {
  string LockKey { get; }
  TimeSpan Expiration { get; }
  TimeSpan WaitTime { get; }
}
