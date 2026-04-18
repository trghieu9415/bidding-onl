using L2.Application.Exceptions;
using L2.Application.Repositories.Read;
using MediatR;

namespace L2.Application.UseCases.Transactions.Queries.GetBidderOrder;

public class GetBidderOrderHandler(
  IOrderReadRepository orderReadRepository
) : IRequestHandler<GetBidderOrderQuery, GetBidderOrderResult> {
  public async Task<GetBidderOrderResult> Handle(GetBidderOrderQuery request, CancellationToken ct) {
    var (order, payments) = await orderReadRepository.GetOrderPaymentByIdAsync(request.Id, ct);
    if (order == null || order.BidderId != request.UserId) {
      throw new WorkflowException("Không tìm thấy đơn hàng", 404);
    }

    return new GetBidderOrderResult(order, payments);
  }
}
