using L1.Core.Domain.Transaction.Entities;
using L2.Application.DTOs;
using L2.Application.Exceptions;
using L2.Application.Repositories;
using L2.Application.Repositories.Read;
using MediatR;

namespace L2.Application.UseCases.Transactions.Queries.GetOrder;

public class GetOrderHandler(
  IReadRepository<Order, OrderDto> orderReadRepository,
  IPaymentReadRepository paymentReadRepository
) : IRequestHandler<GetOrderQuery, GetOrderResult> {
  public async Task<GetOrderResult> Handle(GetOrderQuery request, CancellationToken ct) {
    var order = await orderReadRepository.GetByIdAsync(request.Id, ct)
                ?? throw new WorkflowException("Đơn hàng không tồn tại", 404);
    var payment = await paymentReadRepository.GetByOrderIdAsync(order.Id, ct);
    return new GetOrderResult(order, payment);
  }
}
