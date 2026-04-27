using L1.Core.Domain.Transaction.Entities;
using L1.Core.Domain.Transaction.Enums;
using L2.Application.Ports.Gateway;
using L2.Application.Repositories;
using MediatR;

namespace L2.Application.UseCases.Transactions.Commands.ProcessPayment;

public class ProcessPaymentHandler(
  IRepository<Payment> paymentRepository,
  IGatewayFactory gatewayFactory
) : IRequestHandler<ProcessPaymentCommand, bool> {
  public async Task<bool> Handle(ProcessPaymentCommand request, CancellationToken ct) {
    var data = request.Data;

    var paymentGateway = gatewayFactory.CreatePaymentGateway(data.Method);
    var webhookPayload = paymentGateway.ToWebhookPayload(data.Payload);
    var (isSucceed, transactionId) = await paymentGateway.VerifyWebhookPayment(webhookPayload, ct);

    if (!isSucceed || string.IsNullOrEmpty(transactionId)) {
      return false;
    }

    var payment = await paymentRepository.GetFirstAsync(p => transactionId == p.TransactionId, ct);
    if (payment == null) {
      return true;
    }

    if (payment.Status == PaymentStatus.Succeeded) {
      return true;
    }

    payment.MarkAsCompleted(transactionId);
    return true;
  }
}
