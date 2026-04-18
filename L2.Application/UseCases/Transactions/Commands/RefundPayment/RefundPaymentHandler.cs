using L1.Core.Domain.Transaction.Entities;
using L2.Application.Exceptions;
using L2.Application.Repositories;
using L2.Application.Repositories.Read;
using MediatR;

namespace L2.Application.UseCases.Transactions.Commands.RefundPayment;

public class RefundPaymentHandler(
  IRepository<Payment> paymentRepository,
  IOrderReadRepository orderReadRepository
) : IRequestHandler<RefundPaymentCommand, bool> {
  public async Task<bool> Handle(RefundPaymentCommand request, CancellationToken ct) {
    var payment =
      await paymentRepository.GetByIdAsync(request.Id, ct)
      ?? throw new WorkflowException("Không tìm thấy thanh toán", 404);

    var order = (await orderReadRepository.GetFirstAsync(o => o.Id == payment.OrderId, ct))!;
    if (order.BidderId != request.UserId) {
      throw new WorkflowException("Bạn không có quyền hoàn trả đơn hàng này", 403);
    }

    payment.Refund();
    await paymentRepository.UpdateAsync(payment, ct);
    return true;
  }
}
