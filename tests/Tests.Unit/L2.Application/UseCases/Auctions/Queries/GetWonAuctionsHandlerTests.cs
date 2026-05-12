using System.Linq.Expressions;
using FluentAssertions;
using L1.Core.Domain.Bidding.Entities;
using L1.Core.Domain.Bidding.Enums;
using L2.Application.DTOs;
using L2.Application.Filters;
using L2.Application.Repositories;
using L2.Application.UseCases.Auctions.Queries.GetWonAuctions;
using NSubstitute;
using Tests.Common.Builders;
using Xunit;

namespace Tests.Unit.L2.Application.UseCases.Auctions.Queries;

public class GetWonAuctionsHandlerTests {
  private readonly IReadRepository<Auction, AuctionDto> _readRepository = Substitute.For<IReadRepository<Auction, AuctionDto>>();
  private readonly GetWonAuctionsHandler _sut;

  public GetWonAuctionsHandlerTests() {
    _sut = new GetWonAuctionsHandler(_readRepository);
  }

  [Fact]
  public async Task Handle_Should_QueryWonAuctions_And_ReturnMeta() {
    var userId = Guid.NewGuid();
    var filter = new AuctionFilter { Page = 1, PerPage = 5 };
    var query = new GetWonAuctionsQuery(userId, filter);
    var auctions = new List<AuctionDto> {
      CreateAuctionDto(Guid.NewGuid())
    };
    Expression<Func<Auction, bool>>? capturedCriteria = null;

    _readRepository
      .GetAsync(
        Arg.Do<Expression<Func<Auction, bool>>?>(x => capturedCriteria = x),
        filter,
        CancellationToken.None
      )
      .Returns((6, auctions));

    var result = await _sut.Handle(query, CancellationToken.None);

    result.Auctions.Should().BeSameAs(auctions);
    result.Meta.Page.Should().Be(1);
    result.Meta.PerPage.Should().Be(5);
    result.Meta.Total.Should().Be(6);
    result.Meta.TotalPages.Should().Be(2);
    result.Meta.HasNextPage.Should().BeTrue();
    capturedCriteria.Should().NotBeNull();

    var predicate = capturedCriteria!.Compile();
    predicate(CreateEndedSoldAuctionWonBy(userId)).Should().BeTrue();
    predicate(CreateEndedSoldAuctionWonBy(Guid.NewGuid())).Should().BeFalse();
    predicate(CreateEndedUnsoldAuctionWithBidFrom(userId)).Should().BeFalse();
  }

  private static AuctionDto CreateAuctionDto(Guid id) {
    return new AuctionDto {
      Id = id,
      CatalogItemId = Guid.NewGuid(),
      Status = AuctionStatus.EndedSold,
      CurrentPrice = 1200,
      StepPrice = 100,
      ReservePrice = 1000
    };
  }

  private static Auction CreateEndedSoldAuctionWonBy(Guid bidderId) {
    var auction = new AuctionBuilder()
      .WithPrices(1000, 100, 1000)
      .Build();

    auction.Start();
    auction.PlaceBid(bidderId, "Winner", 1000);
    auction.End();

    return auction;
  }

  private static Auction CreateEndedUnsoldAuctionWithBidFrom(Guid bidderId) {
    var auction = new AuctionBuilder()
      .WithPrices(1000, 100, 2000)
      .Build();

    auction.Start();
    auction.PlaceBid(bidderId, "Bidder", 1000);
    auction.End();

    return auction;
  }
}
