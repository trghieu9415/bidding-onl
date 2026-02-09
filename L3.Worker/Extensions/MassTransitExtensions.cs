using L2.Application.Ports.Messaging;
using L3.Infrastructure.Persistence;
using L3.Worker.Adapters.Messaging;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace L3.Worker.Extensions;

public static class MassTransitExtensions {
  public static IServiceCollection AddCustomMassTransit(this IServiceCollection services) {
    services.AddMassTransit(x => {
      x.AddConsumers(typeof(WorkerConfiguration).Assembly);

      x.AddQuartzConsumers();
      x.AddPublishMessageScheduler();

      x.AddEntityFrameworkOutbox<AppDbContext>(o => {
        o.UsePostgres();
        o.UseBusOutbox();
        o.DuplicateDetectionWindow = TimeSpan.FromMinutes(30);
      });

      // TODO: Chuyển sang RabbitMq
      x.UsingInMemory((context, cfg) => {
        cfg.UsePublishMessageScheduler();
        cfg.UseMessageRetry(r =>
          r.Incremental(3, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(2)));

        cfg.ConfigureEndpoints(context);
      });
    });

    services.AddScoped<IEventDispatcher, MassTransitEventDispatcher>();
    return services;
  }
}
