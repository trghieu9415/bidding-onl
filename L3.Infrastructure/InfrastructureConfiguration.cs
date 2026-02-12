using L2.Application.Ports.Logging;
using L3.Infrastructure.Adapters.Logging;
using L3.Infrastructure.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace L3.Infrastructure;

public static class InfrastructureConfiguration {
  public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config) {
    services
      .AddConfigurationOptions(config)
      .AddPostgresPersistence(config)
      .AddIdentityInfrastructure()
      .AddDistributedInfrastructure()
      .AddMediatorPipeline()
      .AddExternalServices();

    services.AddScoped(typeof(IAppLogger<>), typeof(LoggerAdapter<>));

    return services;
  }
}
