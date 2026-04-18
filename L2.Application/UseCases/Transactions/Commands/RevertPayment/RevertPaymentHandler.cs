using L1.Core.Domain.Transaction.Entities;
using L2.Application.Exceptions;
using L2.Application.Repositories;
using MediatR;

namespace L2.Application.UseCases.Transactions.Commands.RevertPayment;

public class RevertPaymentHandler(
  IRepository<Payment> paymentRepository
) : IRequestHandler<RevertPaymentCommand, bool> {
  public async Task<bool> Handle(RevertPaymentCommand request, CancellationToken ct) {
    var payment =
      await paymentRepository.GetByIdAsync(request.Id, ct)
      ?? throw new WorkflowException("Không tìm thấy thanh toán", 404);

    payment.Refund();
    await paymentRepository.UpdateAsync(payment, ct);
    return true;
  }
}
