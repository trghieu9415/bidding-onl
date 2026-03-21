using L3.Worker.BackgroundJobs;
using Quartz;

namespace L3.Worker;

public static class JobRegistration {
  public static void RegisterApplicationJobs(this IServiceCollectionQuartzConfigurator q) {
    // Startup Job
    q.ScheduleStartupJob<StartupSyncJob>(TimeSpan.FromSeconds(30));
    // Mỗi 1 Tiếng
    q.ScheduleCronJob<UnpaidWinnerTimeoutJob>("0 0/1 * * * ?");
    // 02:00 Sáng
    q.ScheduleCronJob<ImageCleanupJob>("0 0 2 * * ?", "Dọn dẹp ảnh rác");
    // 03:00 Sáng
    q.ScheduleCronJob<SoftDeletePurgeJob>("0 0 3 * * ?", "Xóa vĩnh viễn dữ liệu soft delete");
  }

  // NOTE: ========== [Helper Methods] ==========
  private static TimeZoneInfo GetVnTimeZone() {
    var timeZoneId = Environment.OSVersion.Platform == PlatformID.Win32NT
      ? "SE Asia Standard Time"
      : "Asia/Ho_Chi_Minh";
    return TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
  }

  private static void AddJobAndTrigger<TJob>(
    this IServiceCollectionQuartzConfigurator q,
    Action<ITriggerConfigurator> configureTrigger,
    string description = ""
  ) where TJob : IJob {
    var jobName = typeof(TJob).Name;
    var jobKey = new JobKey(jobName);

    q.AddJob<TJob>(opts => opts.WithIdentity(jobKey).WithDescription(description));
    q.AddTrigger(opts => {
      opts.ForJob(jobKey)
        .WithIdentity($"{jobName}-Trigger");
      configureTrigger(opts);
    });
  }

  private static void ScheduleCronJob<TJob>(
    this IServiceCollectionQuartzConfigurator q,
    string cronExpression,
    string description = ""
  ) where TJob : IJob {
    q.AddJobAndTrigger<TJob>(
      trigger => trigger.WithCronSchedule(
        cronExpression, x => x.InTimeZone(GetVnTimeZone())
      ), description
    );
  }

  private static void ScheduleStartupJob<TJob>(
    this IServiceCollectionQuartzConfigurator q,
    TimeSpan delay,
    string description = ""
  ) where TJob : IJob {
    q.AddJobAndTrigger<TJob>(
      trigger => trigger.StartAt(DateBuilder.EvenSecondDate(DateTimeOffset.Now.Add(delay))),
      description
    );
  }

  private static void SchedulePeriodicJob<TJob>(
    this IServiceCollectionQuartzConfigurator q,
    TimeSpan interval,
    string description = ""
  ) where TJob : IJob {
    q.AddJobAndTrigger<TJob>(
      trigger => trigger.StartNow()
        .WithSimpleSchedule(x => x.WithInterval(interval).RepeatForever()),
      description
    );
  }
}
