using FluentAssertions;
using L1.Core.Domain.Bidding.Entities;
using L1.Core.Domain.Bidding.Enums;
using L2.Application.DTOs;
using L2.Application.Exceptions;
using L2.Application.Repositories;
using L2.Application.UseCases.Auctions.Queries.GetAuction;
using NSubstitute;
using Xunit;

namespace Tests.Unit.L2.Application.UseCases.Auctions.Queries;

public class GetAuctionHandlerTests {
  private readonly IReadRepository<Auction, AuctionDto> _readRepository = Substitute.For<IReadRepository<Auction, AuctionDto>>();
  private readonly GetAuctionHandler _sut;

  public GetAuctionHandlerTests() {
    _sut = new GetAuctionHandler(_readRepository);
  }

  [Fact]
  public async Task Handle_Should_ThrowWorkflowException_When_AuctionNotFound() {
    var query = new GetAuctionQuery(Guid.NewGuid());

    _readRepository.GetByIdAsync(query.Id, CancellationToken.None)
      .Returns((AuctionDto?)null);

    var act = async () => await _sut.Handle(query, CancellationToken.None);

    var exception = await act.Should().ThrowAsync<WorkflowException>();
    exception.Which.StatusCode.Should().Be(404);
    exception.Which.Message.Should().Be("Không tìm thấy thông tin đấu giá");
  }

  [Fact]
  public async Task Handle_Should_ReturnAuction_When_AuctionExists() {
    var query = new GetAuctionQuery(Guid.NewGuid());
    var auction = CreateAuctionDto(query.Id);

    _readRepository.GetByIdAsync(query.Id, CancellationToken.None)
      .Returns(auction);

    var result = await _sut.Handle(query, CancellationToken.None);

    result.Auction.Should().BeSameAs(auction);
  }

  private static AuctionDto CreateAuctionDto(Guid id) {
    return new AuctionDto {
      Id = id,
      CatalogItemId = Guid.NewGuid(),
      Status = AuctionStatus.Scheduled,
      CurrentPrice = 1000,
      StepPrice = 100,
      ReservePrice = 1500
    };
  }
}
