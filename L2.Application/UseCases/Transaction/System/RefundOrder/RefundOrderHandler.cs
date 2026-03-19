using L1.Core.Domain.Transaction.Entities;
using L2.Application.Exceptions;
using L2.Application.Repositories;
using MediatR;

namespace L2.Application.UseCases.Transaction.System.RefundOrder;

public class RefundOrderHandler(
  IRepository<Order> orderRepository
) : IRequestHandler<RefundOrderCommand, bool> {
  public async Task<bool> Handle(RefundOrderCommand request, CancellationToken ct) {
    var order =
      await orderRepository.GetByIdAsync(request.Id, ct)
      ?? throw new WorkflowException("Đơn hàng không tồn tại", 404);

    order.Refund();
    await orderRepository.UpdateAsync(order, ct);
    return true;
  }
}
