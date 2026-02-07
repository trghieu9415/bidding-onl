using L2.Application;
using L3.Infrastructure.Behaviors;
using Microsoft.Extensions.DependencyInjection;

namespace L3.Infrastructure.Extensions;

public static class MediatorExtensions {
  public static IServiceCollection AddMediatorPipeline(this IServiceCollection services) {
    var applicationAssembly = typeof(IApplicationMarker).Assembly;

    services.AddMediatR(cfg => {
      cfg.RegisterServicesFromAssembly(applicationAssembly);

      cfg.AddOpenBehavior(typeof(LockBehavior<,>));
      cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
      cfg.AddOpenBehavior(typeof(TransactionBehavior<,>));
    });

    services.AddAutoMapper(_ => {}, applicationAssembly);
    return services;
  }
}
