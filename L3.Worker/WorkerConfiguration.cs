using L3.Infrastructure.Persistence;
using L3.Worker.BackgroundJobs;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace L3.Worker;

public static class WorkerConfiguration {
  public static IServiceCollection AddWorker(this IServiceCollection services, IConfiguration config) {
    var connectionString =
      config.GetConnectionString("DefaultConnection")
      ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    services.AddQuartz(q => {
      q.UsePersistentStore(s => {
        s.UsePostgres(connectionString);
        s.UseNewtonsoftJsonSerializer();
        s.UseClustering();
      });

      // NOTE: ========== [Tự khởi động] ==========
      var startupSyncKey = new JobKey("StartupSyncJob");
      q.AddJob<StartupSyncJob>(opts => opts.WithIdentity(startupSyncKey));
      q.AddTrigger(opts => opts.ForJob(startupSyncKey).StartNow());

      // NOTE: ========== [02:00 Sáng] ==========
      var imageCleanupKey = new JobKey("ImageCleanupJob");
      q.AddJob<ImageCleanupJob>(opts => opts.WithIdentity(imageCleanupKey));
      q.AddTrigger(opts => opts.ForJob(imageCleanupKey).WithCronSchedule("0 0 2 * * ?"));

      // NOTE: ========== [03:00 Sáng] ==========
      var purgeKey = new JobKey("SoftDeletePurgeJob");
      q.AddJob<SoftDeletePurgeJob>(opts => opts.WithIdentity(purgeKey));
      q.AddTrigger(opts => opts.ForJob(purgeKey).WithCronSchedule("0 0 3 * * ?"));

      // NOTE: ========== [Mỗi 1 tiếng] ==========
      var timeoutKey = new JobKey("UnpaidWinnerTimeoutJob");
      q.AddJob<UnpaidWinnerTimeoutJob>(opts => opts.WithIdentity(timeoutKey));
      q.AddTrigger(opts => opts.ForJob(timeoutKey)
        .WithSimpleSchedule(x => x.WithIntervalInHours(1).RepeatForever()));
    });

    services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

    services.AddMassTransit(x => {
      x.AddConsumers(typeof(WorkerConfiguration).Assembly);
      x.AddQuartzConsumers();
      x.AddPublishMessageScheduler();

      x.AddEntityFrameworkOutbox<AppDbContext>(o => {
        o.UsePostgres();
        o.UseBusOutbox();
      });

      x.UsingInMemory((context, cfg) => {
        cfg.UsePublishMessageScheduler();
        cfg.UseMessageRetry(r => {
          r.Incremental(3, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2));
        });
        cfg.ConfigureEndpoints(context);
      });
    });

    return services;
  }
}
