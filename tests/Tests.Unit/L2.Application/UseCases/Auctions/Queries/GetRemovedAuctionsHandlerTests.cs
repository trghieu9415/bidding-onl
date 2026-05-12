using FluentAssertions;
using L1.Core.Domain.Bidding.Entities;
using L1.Core.Domain.Bidding.Enums;
using L2.Application.DTOs;
using L2.Application.Filters;
using L2.Application.Repositories;
using L2.Application.UseCases.Auctions.Queries.GetRemovedAuctions;
using NSubstitute;
using Xunit;

namespace Tests.Unit.L2.Application.UseCases.Auctions.Queries;

public class GetRemovedAuctionsHandlerTests {
  private readonly IReadRepository<Auction, AuctionDto> _readRepository = Substitute.For<IReadRepository<Auction, AuctionDto>>();
  private readonly GetRemovedAuctionsHandler _sut;

  public GetRemovedAuctionsHandlerTests() {
    _sut = new GetRemovedAuctionsHandler(_readRepository);
  }

  [Fact]
  public async Task Handle_Should_ReturnRemovedAuctions_And_Meta() {
    var filter = new AuctionFilter { Page = 2, PerPage = 10 };
    var query = new GetRemovedAuctionsQuery(filter);
    var auctions = new List<AuctionDto> {
      CreateAuctionDto(Guid.NewGuid())
    };

    _readRepository.GetDeletedAsync(filter: filter, ct: CancellationToken.None)
      .Returns((25, auctions));

    var result = await _sut.Handle(query, CancellationToken.None);

    result.Auctions.Should().BeSameAs(auctions);
    result.Meta.Page.Should().Be(2);
    result.Meta.PerPage.Should().Be(10);
    result.Meta.Total.Should().Be(25);
    result.Meta.TotalPages.Should().Be(3);
    result.Meta.HasPagination.Should().BeTrue();
    result.Meta.HasPreviousPage.Should().BeTrue();
    result.Meta.HasNextPage.Should().BeTrue();

    await _readRepository.Received(1).GetDeletedAsync(filter: filter, ct: CancellationToken.None);
  }

  private static AuctionDto CreateAuctionDto(Guid id) {
    return new AuctionDto {
      Id = id,
      CatalogItemId = Guid.NewGuid(),
      Status = AuctionStatus.Canceled,
      CurrentPrice = 1000,
      StepPrice = 100,
      ReservePrice = 1500
    };
  }
}
