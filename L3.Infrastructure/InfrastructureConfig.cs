using L3.Infrastructure.Extensions;
using L3.Infrastructure.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace L3.Infrastructure;

public static class InfrastructureConfig {
  public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config) {
    services
      .AddPostgresPersistence(config)
      .AddIdentityInfrastructure()
      .AddDistributedInfrastructure(config)
      .AddMediatorPipeline()
      .AddExternalServices();

    return services;
  }

  private static IServiceCollection AddConfigurationOptions(
    this IServiceCollection services,
    IConfiguration config
  ) {
    services.AddOptions<JwtOptions>()
      .Bind(config.GetSection(JwtOptions.SectionName))
      .ValidateDataAnnotations()
      .ValidateOnStart();

    services.AddOptions<RedisOptions>()
      .Bind(config.GetSection(RedisOptions.SectionName))
      .ValidateDataAnnotations()
      .ValidateOnStart();
    return services;
  }
}
