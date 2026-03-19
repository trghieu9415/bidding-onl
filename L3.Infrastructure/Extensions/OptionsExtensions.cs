using L3.Infrastructure.Configs;
using L3.Infrastructure.Configs.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace L3.Infrastructure.Extensions;

public static class OptionsExtensions {
  public static IServiceCollection AddConfigurationOptions(
    this IServiceCollection services,
    IConfiguration config
  ) {
    services.RegisterOption<JwtOptions>(config);
    services.RegisterOption<RedisOptions>(config);
    services.RegisterOption<EmailOptions>(config);
    services.RegisterOption<RabbitMqOptions>(config);
    services.RegisterOption<S3Options>(config);
    services.RegisterOption<StripeOptions>(config);
    services.RegisterOption<PayPalOptions>(config);

    return services;
  }

  private static void RegisterOption<TOptions>(this IServiceCollection services, IConfiguration config)
    where TOptions : class, IOptionSection {
    var sectionName = typeof(TOptions).GetProperty("SectionName")?.GetValue(null) as string;

    services.AddOptions<TOptions>()
      .Bind(config.GetSection(sectionName!))
      .ValidateDataAnnotations()
      .ValidateOnStart();

    services.AddSingleton(resolver =>
      resolver.GetRequiredService<IOptions<TOptions>>().Value);
  }
}
