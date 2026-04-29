using L1.Core.Domain.Transaction.Enums;

namespace L2.Application.Ports.Gateway;

public interface IGatewayFactory {
  IPaymentGateway CreatePaymentGateway(PaymentMethod method);
}
