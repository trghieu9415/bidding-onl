using FluentValidation;
using L2.Application;
using L2.Application.Behaviors;
using L2.Application.Ports.Logging;
using L3.Infrastructure.Adapters.Logging;
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

    services.AddValidatorsFromAssembly(applicationAssembly);
    services.AddSingleton(typeof(IBusinessLogger<>), typeof(BusinessLogger<>));
    services.AddSingleton(typeof(ISystemLogger<>), typeof(SystemLogger<>));
    return services;
  }
}
