using L1.Core.Domain.Transaction.Entities;
using L1.Core.Domain.Transaction.Enums;
using L2.Application.Ports.Gateway;
using L2.Application.Repositories;
using MediatR;

namespace L2.Application.UseCases.Transactions.Commands.VerifyPayment;

public class VerifyClientPaymentHandler(
  IRepository<Payment> paymentRepository,
  IGatewayFactory gatewayFactory
) : IRequestHandler<VerifyClientPaymentCommand, bool> {
  public async Task<bool> Handle(VerifyClientPaymentCommand request, CancellationToken ct) {
    var data = request.Data;
    var payment = await paymentRepository.GetByIdAsync(data.Id, ct);
    if (payment == null) {
      return false;
    }

    if (payment.Status == PaymentStatus.Succeeded) {
      return true;
    }

    var paymentGateway = gatewayFactory.CreatePaymentGateway(payment.Method);
    var clientPayload = paymentGateway.ToClientPayload(data.Payload);
    var (isSucceed, transactionId) = await paymentGateway.VerifyClientPayment(clientPayload, ct);

    if (isSucceed) {
      payment.MarkAsCompleted(transactionId);
    } else {
      payment.MarkAsFailed();
    }

    return true;
  }
}
