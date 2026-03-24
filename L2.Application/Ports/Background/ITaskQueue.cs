using System.Linq.Expressions;

namespace L2.Application.Ports.Background;

public interface ITaskQueue {
  void Queue<T>(Expression<Func<T, Task>> workItem);
}
