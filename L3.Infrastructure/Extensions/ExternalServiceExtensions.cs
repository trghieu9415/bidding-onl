using L2.Application.Ports.Gateways;
using L2.Application.Ports.Search;
using L2.Application.Ports.Storage;
using L3.Infrastructure.Adapters.Gateways;
using L3.Infrastructure.Adapters.Search;
using L3.Infrastructure.Adapters.Storage;
using L3.Infrastructure.Sieve;
using Microsoft.Extensions.DependencyInjection;
using Sieve.Services;

namespace L3.Infrastructure.Extensions;

public static class ExternalServiceExtensions {
  public static IServiceCollection AddExternalServices(this IServiceCollection services) {
    // Search
    services.AddScoped<IAuctionSearchService, PostgresAuctionSearchService>();

    // Others
    services.AddScoped<ISieveProcessor, AppSieveProcessor>();
    services.AddScoped<IEmailService, EmailService>();
    services.AddScoped<IBinaryStorage, LocalBinaryStorage>();
    services.AddScoped<IEventDispatcher, MassTransitEventDispatcher>();

    return services;
  }
}
