using L1.Core.Domain.Transaction.Enums;
using L2.Application.Ports.Gateway;
using Microsoft.Extensions.DependencyInjection;

namespace L3.Infrastructure.Adapters.Gateway;

public class GatewayFactory(IServiceProvider serviceProvider) : IGatewayFactory {
  public IPaymentGateway CreatePaymentGateway(PaymentMethod method) {
    return serviceProvider.GetRequiredKeyedService<IPaymentGateway>(method);
  }
}
