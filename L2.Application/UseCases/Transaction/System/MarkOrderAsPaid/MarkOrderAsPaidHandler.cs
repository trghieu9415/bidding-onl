using L1.Core.Domain.Transaction.Entities;
using L2.Application.Exceptions;
using L2.Application.Repositories;
using MediatR;

namespace L2.Application.UseCases.Transaction.System.MarkOrderAsPaid;

public class MarkOrderAsPaidHandler(
  IRepository<Order> orderRepository
) : IRequestHandler<MarkOrderAsPaidCommand, bool> {
  public async Task<bool> Handle(MarkOrderAsPaidCommand request, CancellationToken ct) {
    var order =
      await orderRepository.GetByIdAsync(request.Id, ct)
      ?? throw new WorkflowException("Đơn hàng không tồn tại", 404);

    order.MarkAsPaid();

    await orderRepository.UpdateAsync(order, ct);
    return true;
  }
}
