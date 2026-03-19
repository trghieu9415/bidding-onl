using L1.Core.Domain.Transaction.Entities;
using L2.Application.DTOs;
using L2.Application.Exceptions;
using L2.Application.Ports.Security;
using L2.Application.Repositories;
using MediatR;

namespace L2.Application.UseCases.Transaction.Bidder.GetOrder;

public class GetOrderHandler(
  IReadRepository<Order, OrderDto> readRepository,
  ICurrentUser currentUser
) : IRequestHandler<GetOrderQuery, GetOrderResult> {
  public async Task<GetOrderResult> Handle(GetOrderQuery request, CancellationToken ct) {
    var order = await readRepository.GetByIdAsync(request.Id, ct)
                ?? throw new WorkflowException("Đơn hàng không tồn tại", 404);

    return order.BidderId != currentUser.Id
      ? throw new WorkflowException("Bạn không có quyền truy cập đơn hàng này", 403)
      : new GetOrderResult(order);
  }
}
