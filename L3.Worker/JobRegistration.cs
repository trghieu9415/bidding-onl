using L3.Worker.BackgroundJobs;
using Quartz;

namespace L3.Worker;

public static class JobRegistration {
  public static void RegisterApplicationJobs(this IServiceCollectionQuartzConfigurator q) {
    var startupKey = new JobKey("StartupSyncJob");
    q.AddJob<StartupSyncJob>(opts => opts.WithIdentity(startupKey));
    q.AddTrigger(opts => opts
      .ForJob(startupKey)
      .StartAt(DateBuilder.EvenSecondDate(DateTimeOffset.Now.AddSeconds(30)))
    );

    var timeoutKey = new JobKey("UnpaidWinnerTimeoutJob");
    q.AddJob<UnpaidWinnerTimeoutJob>(opts => opts.WithIdentity(timeoutKey));
    q.AddTrigger(opts => opts
      .ForJob(timeoutKey)
      .WithSimpleSchedule(x => x.WithIntervalInHours(1).RepeatForever()));

    // 02:00 Sáng
    q.ScheduleJob<ImageCleanupJob>("0 0 2 * * ?", "Dọn dẹp ảnh rác");
    // 03:00 Sáng
    q.ScheduleJob<SoftDeletePurgeJob>("0 0 3 * * ?", "Xóa vĩnh viễn dữ liệu soft delete");
  }

  // NOTE: ========== [Helper Methods] ==========
  private static TimeZoneInfo GetVnTimeZone() {
    var timeZoneId = Environment.OSVersion.Platform == PlatformID.Win32NT
      ? "SE Asia Standard Time"
      : "Asia/Ho_Chi_Minh";
    return TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
  }

  private static void ScheduleJob<TJob>(this IServiceCollectionQuartzConfigurator q, string cronExpression,
    string description = "")
    where TJob : IJob {
    var jobKey = new JobKey(typeof(TJob).Name);
    q.AddJob<TJob>(opts => opts.WithIdentity(jobKey).WithDescription(description));

    q.AddTrigger(opts => opts
      .ForJob(jobKey)
      .WithIdentity($"{typeof(TJob).Name}-Trigger")
      .WithCronSchedule(cronExpression, x => x.InTimeZone(GetVnTimeZone()))
    );
  }
}
