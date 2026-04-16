using L3.Worker.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace L3.Worker;

public static class WorkerConfiguration {
  public static IServiceCollection AddWorker(this IServiceCollection services, IConfiguration config) {
    services
      .AddHangfireInfrastructure(config)
      .AddCustomMassTransit();

    return services;
  }
}
