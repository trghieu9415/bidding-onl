using L2.Application.Ports.Messaging;
using L3.Infrastructure.Options;
using L3.Infrastructure.Persistence;
using L3.Worker.Adapters.Notification;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace L3.Worker.Extensions;

public static class MassTransitExtensions {
  public static IServiceCollection AddCustomMassTransit(this IServiceCollection services, IConfiguration config) {
    var rabbitMqSettings = config.GetSection(RabbitMqSettings.SectionName).Get<RabbitMqSettings>()!;

    services.AddMassTransit(x => {
      x.AddConsumers(typeof(WorkerConfiguration).Assembly);

      x.AddHangfireConsumers();
      x.AddPublishMessageScheduler();

      x.AddEntityFrameworkOutbox<AppDbContext>(o => {
        o.UsePostgres();
        o.UseBusOutbox();
        o.DuplicateDetectionWindow = TimeSpan.FromMinutes(30);
      });

      x.UsingRabbitMq((context, cfg) => {
        cfg.Host(rabbitMqSettings.Host, rabbitMqSettings.VirtualHost, h => {
          h.Username(rabbitMqSettings.Username);
          h.Password(rabbitMqSettings.Password);
        });

        cfg.UsePublishMessageScheduler();

        // Retry
        cfg.UseMessageRetry(r =>
          r.Exponential(4, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(2))
        );

        // Redelivery
        cfg.UseDelayedRedelivery(r => {
          r.Intervals(
            TimeSpan.FromMinutes(5),
            TimeSpan.FromMinutes(15),
            TimeSpan.FromMinutes(30));
        });

        // Circuit Breaker
        cfg.UseCircuitBreaker(cb => {
          cb.TrackingPeriod = TimeSpan.FromMinutes(1);
          cb.TripThreshold = 15;
          cb.ActiveThreshold = 10;
          cb.ResetInterval = TimeSpan.FromMinutes(5);
        });

        cfg.ConfigureEndpoints(context);
      });
    });

    services.AddScoped<IEventDispatcher, MassTransitEventDispatcher>();
    return services;
  }
}
