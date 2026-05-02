using L2.Application.DTOs;
using L2.Application.Models;
using L2.Application.UseCases.Bids.Queries.GetBidHistory;
using Tests.Unit.L2.Application.UseCases.TestDoubles;
using Xunit;

namespace Tests.Unit.L2.Application.UseCases.Bids;

public class GetBidHistoryHandlerTests {
  [Fact]
  public async Task Handle_ReturnsBidsAndMeta() {
    var bids = new List<BidDto> { new() { Id = Guid.NewGuid(), Amount = 100m } };
    var readRepo = new StubAuctionReadRepository { GetBidsResult = (5, bids) };
    var handler = new GetBidHistoryHandler(readRepo);

    var result = await handler.Handle(
      new GetBidHistoryQuery(Guid.NewGuid(), 2, 3),
      TestContext.Current.CancellationToken
    );

    Assert.Equal(bids, result.Bids);
    Assert.Equal(Meta.Create(2, 3, 5), result.Meta);
    Assert.Equal(2, readRepo.LastPage);
    Assert.Equal(3, readRepo.LastPerPage);
  }
}
