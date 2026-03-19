using L1.Core.Domain.Transaction.Entities;
using L1.Core.Domain.Transaction.Enums;
using L2.Application.Exceptions;
using L2.Application.Repositories;
using MediatR;

namespace L2.Application.UseCases.Transaction.System.ExpireOrder;

public class ExpireOrderHandler(
  IRepository<Order> orderRepository
) : IRequestHandler<ExpireOrderCommand, bool> {
  public async Task<bool> Handle(ExpireOrderCommand request, CancellationToken ct) {
    var order =
      await orderRepository.GetByIdAsync(request.Id, ct)
      ?? throw new WorkflowException("Đơn hàng không tồn tại", 404);

    if (order.Status == OrderStatus.Confirmed) {
      return false;
    }

    order.Cancel();
    await orderRepository.UpdateAsync(order, ct);
    return true;
  }
}
