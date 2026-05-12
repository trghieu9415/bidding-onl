using FluentAssertions;
using L1.Core.Domain.Bidding.Entities;
using L1.Core.Domain.Catalog.Entities;
using L1.Core.Domain.Catalog.Enums;
using L2.Application.DTOs;
using L2.Application.Exceptions;
using L2.Application.Repositories;
using L2.Application.UseCases.Auctions.Commands.AddAuction;
using NSubstitute;
using Xunit;

namespace Tests.Unit.L2.Application.UseCases.Auctions.Commands;

public class AddAuctionHandlerTests {
  private readonly IRepository<Auction> _auctionRepository = Substitute.For<IRepository<Auction>>();
  private readonly IReadRepository<CatalogItem, CatalogItemDto> _itemRepository =
    Substitute.For<IReadRepository<CatalogItem, CatalogItemDto>>();

  private readonly AddAuctionHandler _sut;

  public AddAuctionHandlerTests() {
    _sut = new AddAuctionHandler(_auctionRepository, _itemRepository);
  }

  [Fact]
  public async Task Handle_Should_ThrowWorkflowException_When_ItemNotFound() {
    var command = new AddAuctionCommand(Guid.NewGuid(), Guid.NewGuid(), 100, 1000);

    _itemRepository.GetByIdAsync(command.CatalogItemId, CancellationToken.None)
      .Returns((CatalogItemDto?)null);

    var act = async () => await _sut.Handle(command, CancellationToken.None);

    var exception = await act.Should().ThrowAsync<WorkflowException>();
    exception.Which.StatusCode.Should().Be(404);
    exception.Which.Message.Should().Be($"Không tìm thấy vật phẩm - Id: {command.CatalogItemId}");
  }

  [Fact]
  public async Task Handle_Should_ThrowWorkflowException_When_ItemIsNotApproved() {
    var command = new AddAuctionCommand(Guid.NewGuid(), Guid.NewGuid(), 100, 1000);
    var item = new CatalogItemDto {
      Id = command.CatalogItemId,
      OwnerId = Guid.NewGuid(),
      Name = "Laptop",
      Description = "Gaming laptop",
      Status = ItemStatus.Pending,
      StartingPrice = 500
    };

    _itemRepository.GetByIdAsync(command.CatalogItemId, CancellationToken.None)
      .Returns(item);

    var act = async () => await _sut.Handle(command, CancellationToken.None);

    var exception = await act.Should().ThrowAsync<WorkflowException>();
    exception.Which.StatusCode.Should().Be(400);
    exception.Which.Message.Should().Be($"Sản phẩm chưa được phê duyệt để sẵn sàng để đấu giá - Id: {item.Id}");
  }

  [Fact]
  public async Task Handle_Should_CreateAuction_When_ItemIsApproved() {
    var command = new AddAuctionCommand(Guid.NewGuid(), Guid.NewGuid(), 100, 1000);
    var ownerId = Guid.NewGuid();
    var createdId = Guid.NewGuid();
    var item = new CatalogItemDto {
      Id = command.CatalogItemId,
      OwnerId = ownerId,
      Name = "Laptop",
      Description = "Gaming laptop",
      Status = ItemStatus.Approval,
      StartingPrice = 500
    };

    _itemRepository.GetByIdAsync(command.CatalogItemId, CancellationToken.None)
      .Returns(item);
    _auctionRepository.CreateAsync(Arg.Any<Auction>(), CancellationToken.None)
      .Returns(createdId);

    var result = await _sut.Handle(command, CancellationToken.None);

    result.Should().Be(createdId);
    await _auctionRepository.Received(1).CreateAsync(
      Arg.Is<Auction>(x =>
        x.CatalogItemId == item.Id &&
        x.SessionId == command.SessionId &&
        x.OwnerId == ownerId &&
        x.CurrentPrice == item.StartingPrice &&
        x.Rules.StepPrice == command.StepPrice &&
        x.Rules.ReservePrice == command.ReservePrice
      ),
      CancellationToken.None
    );
  }
}
