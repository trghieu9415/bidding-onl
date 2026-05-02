using L1.Core.Domain.Bidding.Entities;
using L1.Core.Domain.Transaction.Entities;
using L2.Application.DTOs;
using L2.Application.Repositories.Read;

namespace Tests.Unit.L2.Application.UseCases.TestDoubles;

public sealed class StubAuctionReadRepository : StubReadRepository<Auction, AuctionDto>, IAuctionReadRepository {
  public (int total, List<BidDto> bids) GetBidsResult { get; set; } = (0, []);
  public Guid? LastAuctionId { get; private set; }
  public int LastPage { get; private set; }
  public int LastPerPage { get; private set; }

  public Task<(int total, List<BidDto> bids)> GetBidsAsync(
    Guid auctionId,
    int page = 1,
    int perPage = 10,
    CancellationToken ct = default
  ) {
    LastAuctionId = auctionId;
    LastPage = page;
    LastPerPage = perPage;
    return Task.FromResult(GetBidsResult);
  }
}

public sealed class StubOrderReadRepository : StubReadRepository<Order, OrderDto>, IOrderReadRepository {
  public OrderDto? OrderByAuctionIdResult { get; set; }
  public (OrderDto? order, List<PaymentDto> payments) OrderPaymentsResult { get; set; } = (null, []);

  public Task<OrderDto?> GetByAuctionIdAsync(Guid auctionId, CancellationToken ct = default) {
    return Task.FromResult(OrderByAuctionIdResult);
  }

  public Task<(OrderDto? order, List<PaymentDto> payments)> GetOrderPaymentByIdAsync(
    Guid orderId,
    CancellationToken ct = default
  ) {
    return Task.FromResult(OrderPaymentsResult);
  }
}

public sealed class StubSessionReadRepository : StubReadRepository<AuctionSession, AuctionSessionDto>,
  ISessionReadRepository {
  public List<AuctionSessionDto> CurrentSessionsResult { get; set; } = [];

  public Task<List<AuctionSessionDto>> GetCurrentSessionsAsync(CancellationToken ct = default) {
    return Task.FromResult(CurrentSessionsResult);
  }
}
