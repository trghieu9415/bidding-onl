using L1.Core.Domain.Transaction.Entities;
using L2.Application.DTOs;

namespace L2.Application.Repositories.Read;

public interface IOrderReadRepository : IReadRepository<Order, OrderDto> {
  Task<OrderDto?> GetByAuctionIdAsync(Guid auctionId, CancellationToken ct = default);

  Task<(OrderDto? order, List<PaymentDto> payments)> GetOrderPaymentByIdAsync(Guid orderId,
    CancellationToken ct = default);
}
