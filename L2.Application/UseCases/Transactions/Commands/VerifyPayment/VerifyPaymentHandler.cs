using L1.Core.Domain.Transaction.Entities;
using L2.Application.Ports.Gateway;
using L2.Application.Repositories;
using MediatR;

namespace L2.Application.UseCases.Transactions.Commands.VerifyPayment;

public class VerifyPaymentHandler(
  IRepository<Payment> paymentRepository,
  IGatewayFactory gatewayFactory
) : IRequestHandler<VerifyPaymentCommand, bool> {
  public async Task<bool> Handle(VerifyPaymentCommand request, CancellationToken ct) {
    var data = request.Data;
    var payment = await paymentRepository.GetByIdAsync(data.Id, ct);
    if (payment == null) {
      return false;
    }

    var paymentGateway = gatewayFactory.CreatePaymentGateway(payment.Method);
    var gatewayPayload = paymentGateway.ToGatewayPayload(data.Payload);
    var (isSucceed, transactionId) = await paymentGateway.VerifyPayment(gatewayPayload, ct);

    if (isSucceed) {
      payment.MarkAsCompleted(request.UserId, transactionId);
    } else {
      payment.MarkAsFailed();
    }

    return true;
  }
}
