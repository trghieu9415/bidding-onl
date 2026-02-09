using L3.Infrastructure.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace L3.Infrastructure;

public static class InfrastructureConfig {
  public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config) {
    services
      .AddConfigurationOptions(config)
      .AddPostgresPersistence(config)
      .AddIdentityInfrastructure()
      .AddDistributedInfrastructure()
      .AddMediatorPipeline()
      .AddExternalServices();

    return services;
  }
}
