using System.Linq.Expressions;

namespace L2.Application.Ports.Background;

public interface ITaskQueue {
  string Enqueue<T>(Expression<Func<T, Task>> workItem);
  string Schedule<T>(Expression<Func<T, Task>> workItem, TimeSpan delay);
  string Schedule<T>(Expression<Func<T, Task>> workItem, DateTimeOffset enqueueAt);
  bool Delete(string jobId);
}
