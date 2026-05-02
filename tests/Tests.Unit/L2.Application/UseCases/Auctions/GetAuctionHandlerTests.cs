using L1.Core.Domain.Bidding.Entities;
using L2.Application.DTOs;
using L2.Application.Exceptions;
using L2.Application.UseCases.Auctions.Queries.GetAuction;
using Tests.Unit.L2.Application.UseCases.TestDoubles;
using Xunit;

namespace Tests.Unit.L2.Application.UseCases.Auctions;

public class GetAuctionHandlerTests {
  [Fact]
  public async Task Handle_WhenAuctionMissing_ThrowsWorkflowException() {
    var handler = new GetAuctionHandler(new StubReadRepository<Auction, AuctionDto>());

    var exception = await Assert.ThrowsAsync<WorkflowException>(async () =>
      await handler.Handle(
        new GetAuctionQuery(Guid.NewGuid()),
        TestContext.Current.CancellationToken
      ));

    Assert.Equal(404, exception.StatusCode);
    Assert.Equal("Không tìm thấy thông tin đấu giá", exception.Message);
  }
}
