using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace L3.Worker.Extensions;

public static class HangfireExtensions {
  public static IServiceCollection AddHangfireInfrastructure(this IServiceCollection services, IConfiguration config) {
    var connectionString =
      config.GetConnectionString("DefaultConnection")
      ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

    services.AddHangfire(configuration => configuration
      .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
      .UseSimpleAssemblyNameTypeSerializer()
      .UseRecommendedSerializerSettings()
      .UsePostgreSqlStorage(options =>
        options.UseNpgsqlConnection(connectionString)));

    services.AddHangfireServer();
    return services;
  }
}
