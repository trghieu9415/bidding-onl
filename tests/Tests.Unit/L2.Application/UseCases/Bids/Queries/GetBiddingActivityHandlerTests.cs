using System.Linq.Expressions;
using FluentAssertions;
using L1.Core.Domain.Bidding.Entities;
using L2.Application.DTOs;
using L2.Application.Filters;
using L2.Application.Repositories;
using L2.Application.UseCases.Bids.Queries.GetBiddingActivity;
using NSubstitute;
using Tests.Unit.L2.Application.UseCases;
using Tests.Common.Builders;
using Xunit;

namespace Tests.Unit.L2.Application.UseCases.Bids.Queries;

public class GetBiddingActivityHandlerTests {
  private readonly IReadRepository<Auction, AuctionDto> _auctionReadRepository =
    Substitute.For<IReadRepository<Auction, AuctionDto>>();

  private readonly GetBiddingActivityHandler _sut;

  public GetBiddingActivityHandlerTests() {
    _sut = new GetBiddingActivityHandler(_auctionReadRepository);
  }

  [Fact]
  public async Task Handle_Should_Filter_By_BidderActivity_And_ReturnMeta() {
    var userId = Guid.NewGuid();
    var filter = new AuctionFilter { Page = 1, PerPage = 10 };
    var auctions = new List<AuctionDto> { UseCaseTestData.CreateAuctionDto() };
    var request = new GetBiddingActivityQuery(userId, filter);
    Expression<Func<Auction, bool>>? capturedCriteria = null;

    _auctionReadRepository.GetAsync(
        Arg.Do<Expression<Func<Auction, bool>>?>(x => capturedCriteria = x),
        filter,
        CancellationToken.None
      )
      .Returns((11, auctions));

    var result = await _sut.Handle(request, CancellationToken.None);

    result.Auctions.Should().BeSameAs(auctions);
    result.Meta.Total.Should().Be(11);
    capturedCriteria.Should().NotBeNull();

    var predicate = capturedCriteria!.Compile();
    predicate(CreateAuctionWithBidFrom(userId)).Should().BeTrue();
    predicate(CreateAuctionWithBidFrom(Guid.NewGuid())).Should().BeFalse();
  }

  private static Auction CreateAuctionWithBidFrom(Guid bidderId) {
    var auction = new AuctionBuilder().WithPrices(1000, 100, 1500).Build();
    auction.Start();
    auction.PlaceBid(bidderId, "Bidder", 1000);
    return auction;
  }
}
