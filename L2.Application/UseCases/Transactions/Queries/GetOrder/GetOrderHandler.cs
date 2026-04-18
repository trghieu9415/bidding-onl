using L2.Application.Exceptions;
using L2.Application.Repositories.Read;
using MediatR;

namespace L2.Application.UseCases.Transactions.Queries.GetOrder;

public class GetOrderHandler(
  IOrderReadRepository orderReadRepository
) : IRequestHandler<GetOrderQuery, GetOrderResult> {
  public async Task<GetOrderResult> Handle(GetOrderQuery request, CancellationToken ct) {
    var (order, payments) = await orderReadRepository.GetOrderPaymentByIdAsync(request.Id, ct);
    return order == null
      ? throw new WorkflowException("Không tìm thấy đơn hàng", 404)
      : new GetOrderResult(order, payments);
  }
}
