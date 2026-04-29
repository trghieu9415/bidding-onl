using System.Linq.Expressions;
using Hangfire;
using L2.Application.Ports.Background;

namespace L3.Worker.Adapters.Background;

public class HangfireTaskQueue(IBackgroundJobClient jobClient) : ITaskQueue {
  public string Enqueue<T>(Expression<Func<T, Task>> workItem) {
    return jobClient.Enqueue(workItem);
  }

  public string Schedule<T>(Expression<Func<T, Task>> workItem, TimeSpan delay) {
    return jobClient.Schedule(workItem, delay);
  }

  public string Schedule<T>(Expression<Func<T, Task>> workItem, DateTimeOffset enqueueAt) {
    return jobClient.Schedule(workItem, enqueueAt);
  }

  public bool Delete(string jobId) {
    return jobClient.Delete(jobId);
  }
}
