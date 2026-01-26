using L3.Infrastructure.Persistence;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace L3.Worker;

public static class WorkerConfiguration {
  public static IServiceCollection AddWorker(this IServiceCollection services, IConfiguration config) {
    services.AddMassTransit(x => {
      x.AddConsumers(typeof(WorkerConfiguration).Assembly);

      x.AddEntityFrameworkOutbox<AppDbContext>(o => {
        o.UsePostgres();
        o.UseBusOutbox();
      });

      x.UsingInMemory((context, cfg) => {
        cfg.ConfigureEndpoints(context);
      });
    });


    return services;
  }
}
