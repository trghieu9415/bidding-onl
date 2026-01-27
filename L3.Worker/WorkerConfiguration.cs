using L3.Infrastructure.Persistence;
using L3.Worker.BackgroundJobs;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace L3.Worker;

public static class WorkerConfiguration {
  public static IServiceCollection AddWorker(this IServiceCollection services, IConfiguration config) {
    services.AddQuartz(q => {
      q.UseInMemoryStore();

      var startupSyncKey = new JobKey("StartupSyncJob");
      q.AddJob<StartupSyncJob>(opts => opts.WithIdentity(startupSyncKey));
      q.AddTrigger(opts => opts
        .ForJob(startupSyncKey)
        .WithIdentity("StartupSyncJob-Trigger")
        .StartNow()
      );

      var imageCleanupKey = new JobKey("ImageCleanupJob");
      q.AddJob<ImageCleanupJob>(opts => opts.WithIdentity(imageCleanupKey));
      q.AddTrigger(opts => opts
        .ForJob(imageCleanupKey)
        .WithIdentity("ImageCleanup-Trigger")
        // NOTE: ========== [02:00 Everyday] ==========
        .WithCronSchedule("0 0 2 * * ?"));

      var purgeKey = new JobKey("SoftDeletePurgeJob");
      q.AddJob<SoftDeletePurgeJob>(opts => opts.WithIdentity(purgeKey));
      q.AddTrigger(opts => opts
        .ForJob(purgeKey)
        .WithIdentity("SoftDeletePurge-Trigger")
        // NOTE: ========== [03:00 Everyday] ==========
        .WithCronSchedule("0 0 3 * * ?"));

      var timeoutKey = new JobKey("UnpaidWinnerTimeoutJob");
      q.AddJob<UnpaidWinnerTimeoutJob>(opts => opts.WithIdentity(timeoutKey));
      q.AddTrigger(opts => opts
        .ForJob(timeoutKey)
        .WithIdentity("UnpaidWinner-Trigger")
        // NOTE: ========== [Every 1 hour] ==========
        .WithSimpleSchedule(x => x.WithIntervalInHours(1).RepeatForever()));
    });

    services.AddQuartzHostedService(options => {
      options.WaitForJobsToComplete = true;
    });

    services.AddMassTransit(x => {
      x.AddConsumers(typeof(WorkerConfiguration).Assembly);
      x.AddQuartzConsumers();

      x.AddEntityFrameworkOutbox<AppDbContext>(o => {
        o.UsePostgres();
        o.UseBusOutbox();
      });

      x.UsingInMemory((context, cfg) => {
        cfg.UsePublishMessageScheduler();
        cfg.ConfigureEndpoints(context);
      });
    });

    return services;
  }
}
