using L1.Core.Domain.Transaction.Entities;
using L2.Application.Ports.Gateway;
using L2.Application.Repositories;
using MediatR;

namespace L2.Application.UseCases.Transaction.System.RefundPayment;

public class RefundPaymentHandler(
  IRepository<Payment> paymentRepository,
  IGatewayFactory gatewayFactory
) : IRequestHandler<RefundPaymentCommand, bool> {
  public async Task<bool> Handle(RefundPaymentCommand request, CancellationToken ct) {
    var payment = await paymentRepository.GetByIdAsync(request.Id, ct);
    if (payment == null) {
      return false;
    }

    var paymentGateway = gatewayFactory.CreatePaymentGateway(payment.Method);
    var refunded = await paymentGateway.RefundPayment(payment, ct);
    payment.Refund();
    await paymentRepository.UpdateAsync(payment, ct);
    return refunded;
  }
}
