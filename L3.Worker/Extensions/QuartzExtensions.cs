using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace L3.Worker.Extensions;

public static class QuartzExtensions {
  public static IServiceCollection AddQuartzInfrastructure(this IServiceCollection services, IConfiguration config) {
    var connectionString =
      config.GetConnectionString("DefaultConnection")
      ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

    services.AddQuartz(q => {
      q.UsePersistentStore(s => {
        s.UsePostgres(connectionString);
        s.UseNewtonsoftJsonSerializer();
        s.UseClustering();
      });

      q.RegisterApplicationJobs();
    });

    services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

    return services;
  }
}
