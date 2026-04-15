using L1.Core.Domain.Transaction.Entities;
using L2.Application.Exceptions;
using L2.Application.Models;
using L2.Application.Ports.Security;
using L2.Application.Repositories;
using MediatR;

namespace L2.Application.UseCases.System.MarkOrderAsPaid;

public class MarkOrderAsPaidHandler(
  IRepository<Order> orderRepository,
  IUserService userService
) : IRequestHandler<MarkOrderAsPaidCommand, bool> {
  public async Task<bool> Handle(MarkOrderAsPaidCommand request, CancellationToken ct) {
    var order =
      await orderRepository.GetByIdAsync(request.Id, ct)
      ?? throw new WorkflowException("Đơn hàng không tồn tại", 404);

    var user = await userService.GetByIdAsync(order.BidderId, UserRole.Bidder, ct);

    order.MarkAsPaid(user.Email);
    await orderRepository.UpdateAsync(order, ct);
    return true;
  }
}
