using L1.Core.Domain.Transaction.Entities;
using L2.Application.Ports.Gateway;
using L2.Application.Ports.Security;
using L2.Application.Repositories;
using MediatR;

namespace L2.Application.UseCases.Transaction.Bidder.VerifyPayment;

public class VerifyPaymentHandler(
  IRepository<Payment> paymentRepository,
  IGatewayFactory gatewayFactory,
  ICurrentUser currentUser
) : IRequestHandler<VerifyPaymentCommand, bool> {
  public async Task<bool> Handle(VerifyPaymentCommand request, CancellationToken ct) {
    var payment = await paymentRepository.GetByIdAsync(request.Id, ct);
    if (payment == null) {
      return false;
    }

    var paymentGateway = gatewayFactory.CreatePaymentGateway(payment.Method);
    var gatewayPayload = paymentGateway.ToGatewayPayload(request.Payload);
    var (isSucceed, transactionId) = await paymentGateway.VerifyPayment(gatewayPayload, ct);

    if (isSucceed) {
      payment.MarkAsSucceeded(currentUser.Id, transactionId);
    } else {
      payment.MarkAsFailed();
    }

    return true;
  }
}
