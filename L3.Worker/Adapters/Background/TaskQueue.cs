using System.Linq.Expressions;
using Hangfire;
using L2.Application.Ports.Background;
using L3.Infrastructure.Services.Abstractions;

namespace L3.Worker.Adapters.Background;

public class TaskQueue(IBackgroundJobClient jobClient) : ITaskQueue {
  public void Queue<T>(Expression<Func<T, Task>> workItem) {
    jobClient.Enqueue(workItem);
  }
}
