using System.Linq.Expressions;
using Hangfire;
using L3.Worker.Jobs;
using Microsoft.Extensions.Hosting;

namespace L3.Worker;

public class HangfireBootstrapper(
  IBackgroundJobClient backgroundJobs,
  IRecurringJobManager recurringJobs
) : IHostedService {
  // Cache múi giờ lại để không phải tính toán nhiều lần
  private readonly TimeZoneInfo _vnTimeZone = GetVnTimeZone();

  public Task StartAsync(CancellationToken ct) {
    backgroundJobs.Schedule<StartupSyncJob>(
      job => job.Execute(CancellationToken.None),
      TimeSpan.FromSeconds(30)
    );

    AddRecurringJob<UnpaidWinnerTimeoutJob>(job => job.Execute(ct), Cron.Hourly());
    AddRecurringJob<ImageCleanupJob>(job => job.Execute(), "0 2 * * *");
    AddRecurringJob<SoftDeletePurgeJob>(job => job.Execute(), "0 3 * * *");

    return Task.CompletedTask;
  }

  public Task StopAsync(CancellationToken ct) {
    return Task.CompletedTask;
  }


  private void AddRecurringJob<T>(Expression<Func<T, Task>> methodCall, string cronExpression) {
    recurringJobs.AddOrUpdate(
      typeof(T).Name,
      methodCall,
      cronExpression,
      new RecurringJobOptions { TimeZone = _vnTimeZone }
    );
  }

  private void AddRecurringJob<T>(Expression<Action<T>> methodCall, string cronExpression) {
    recurringJobs.AddOrUpdate(
      typeof(T).Name,
      methodCall,
      cronExpression,
      new RecurringJobOptions { TimeZone = _vnTimeZone }
    );
  }

  private static TimeZoneInfo GetVnTimeZone() {
    var timeZoneId = Environment.OSVersion.Platform == PlatformID.Win32NT
      ? "SE Asia Standard Time"
      : "Asia/Ho_Chi_Minh";
    return TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
  }
}
