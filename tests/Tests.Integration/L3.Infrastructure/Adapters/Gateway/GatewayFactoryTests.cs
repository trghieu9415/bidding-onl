using L1.Core.Domain.Transaction.Enums;
using L2.Application.Ports.Gateway;
using L3.Infrastructure.Adapters.Gateway;
using Microsoft.Extensions.DependencyInjection;
using Tests.Integration.TestSupport;
using Xunit;

namespace Tests.Integration.L3.Infrastructure.Adapters.Gateway;

public class GatewayFactoryTests {
  [Fact]
  public void CreatePaymentGateway_resolves_keyed_gateway() {
    var stripeGateway = new FakePaymentGateway();
    var paypalGateway = new FakePaymentGateway();

    var services = new ServiceCollection();
    services.AddKeyedSingleton<IPaymentGateway>(PaymentMethod.Stripe, stripeGateway);
    services.AddKeyedSingleton<IPaymentGateway>(PaymentMethod.Paypal, paypalGateway);

    var provider = services.BuildServiceProvider();
    var factory = new GatewayFactory(provider);

    var stripe = factory.CreatePaymentGateway(PaymentMethod.Stripe);
    var paypal = factory.CreatePaymentGateway(PaymentMethod.Paypal);

    Assert.Same(stripeGateway, stripe);
    Assert.Same(paypalGateway, paypal);
  }
}
