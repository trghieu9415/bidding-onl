namespace L2.Application.Abstractions;

public interface ILockable {
  string LockKey { get; }
  TimeSpan WaitTime { get; }
}
