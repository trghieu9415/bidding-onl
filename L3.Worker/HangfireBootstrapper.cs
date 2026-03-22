using Hangfire;
using L3.Worker.BackgroundJobs;
using Microsoft.Extensions.Hosting;

namespace L3.Worker;

public class HangfireBootstrapper(
  IBackgroundJobClient backgroundJobs,
  IRecurringJobManager recurringJobs
) : IHostedService {
  public Task StartAsync(CancellationToken cancellationToken) {
    var vnTimeZone = GetVnTimeZone();

    backgroundJobs.Schedule<StartupSyncJob>(
      job => job.Execute(cancellationToken),
      TimeSpan.FromSeconds(30)
    );

    recurringJobs.AddOrUpdate<UnpaidWinnerTimeoutJob>(
      "unpaid-winner-timeout",
      job => job.Execute(cancellationToken),
      Cron.Hourly(),
      new RecurringJobOptions { TimeZone = vnTimeZone }
    );

    recurringJobs.AddOrUpdate<ImageCleanupJob>(
      "image-cleanup-job",
      job => job.Execute(),
      "0 2 * * *",
      new RecurringJobOptions { TimeZone = vnTimeZone }
    );

    recurringJobs.AddOrUpdate<SoftDeletePurgeJob>(
      "soft-delete-purge-job",
      job => job.Execute(),
      "0 3 * * *",
      new RecurringJobOptions { TimeZone = vnTimeZone }
    );

    return Task.CompletedTask;
  }

  public Task StopAsync(CancellationToken cancellationToken) {
    return Task.CompletedTask;
  }

  private static TimeZoneInfo GetVnTimeZone() {
    var timeZoneId = Environment.OSVersion.Platform == PlatformID.Win32NT
      ? "SE Asia Standard Time"
      : "Asia/Ho_Chi_Minh";
    return TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
  }
}
