using L2.Application.Ports.Configs;
using L3.Infrastructure.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace L3.Infrastructure.Extensions;

public static class OptionsExtensions {
  public static IServiceCollection AddConfigurationOptions(
    this IServiceCollection services,
    IConfiguration config
  ) {
    // NOTE: ========== [Options Hạ Tầng] ==========
    services.RegisterOption<JwtSettings>(config);
    services.RegisterOption<RedisSettings>(config);
    services.RegisterOption<EmailSettings>(config);
    services.RegisterOption<RabbitMqSettings>(config);
    services.RegisterOption<S3Settings>(config);
    services.RegisterOption<StripeSettings>(config);
    services.RegisterOption<PaypalSettings>(config);

    return services;
  }

  private static void RegisterOption<TOptions>(
    this IServiceCollection services,
    IConfiguration config
  ) where TOptions : class, IOptionSection {
    var sectionName = typeof(TOptions).GetProperty("SectionName")?.GetValue(null) as string;

    services.AddOptions<TOptions>()
      .Bind(config.GetSection(sectionName!))
      .ValidateDataAnnotations()
      .ValidateOnStart();

    services.AddSingleton(resolver =>
      resolver.GetRequiredService<IOptions<TOptions>>().Value);
  }
}
