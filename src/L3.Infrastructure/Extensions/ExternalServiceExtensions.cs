using L1.Core.Domain.Transaction.Enums;
using L2.Application.Ports.Gateway;
using L2.Application.Ports.Search;
using L3.Infrastructure.Adapters.Gateway;
using L3.Infrastructure.Adapters.Gateway.Transaction;
using L3.Infrastructure.Adapters.Search;
using L3.Infrastructure.Services;
using L3.Infrastructure.Services.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;
using Polly;

namespace L3.Infrastructure.Extensions;

public static class ExternalServiceExtensions {
  public static IServiceCollection AddExternalServices(this IServiceCollection services) {
    services.AddScoped<IEmailService, EmailService>();
    services.AddScoped<IStorageService, S3StorageService>();
    services.AddScoped<ISearchService, PostgresSearchService>();

    services.AddScoped<IGatewayFactory, GatewayFactory>();

    // Transactions
    services.AddKeyedScoped<IPaymentGateway, StripeGateway>(PaymentMethod.Stripe);
    services.AddKeyedScoped<IPaymentGateway, PaypalGateway>(PaymentMethod.Paypal);

    services.AddHttpClient(nameof(PaypalGateway))
      .AddStandardResilienceHandler(ConfigureExternalServicesResilience());

    services.AddAutoMapper(_ => {}, typeof(InfrastructureConfiguration).Assembly);
    return services;
  }

  private static Action<HttpStandardResilienceOptions> ConfigureExternalServicesResilience() {
    return options => {
      // Retry Policy
      options.Retry.MaxRetryAttempts = 3;
      options.Retry.Delay = TimeSpan.FromSeconds(2);
      options.Retry.BackoffType = DelayBackoffType.Exponential;

      // Circuit Breaker
      options.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(30);
      options.CircuitBreaker.FailureRatio = 0.5;
      options.CircuitBreaker.MinimumThroughput = 5;
      options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(10);
      options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(45);
    };
  }
}
