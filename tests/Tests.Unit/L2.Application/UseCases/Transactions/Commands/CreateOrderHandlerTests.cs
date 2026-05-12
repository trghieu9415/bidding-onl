using FluentAssertions;
using L1.Core.Domain.Bidding.Entities;
using L1.Core.Domain.Bidding.Enums;
using L1.Core.Domain.Catalog.Entities;
using L1.Core.Domain.Transaction.Entities;
using L2.Application.DTOs;
using L2.Application.Exceptions;
using L2.Application.Repositories;
using L2.Application.Repositories.Read;
using L2.Application.UseCases.Transactions.Commands.CreateOrder;
using NSubstitute;
using Tests.Common.Builders;
using Tests.Unit.L2.Application.UseCases;
using Xunit;

namespace Tests.Unit.L2.Application.UseCases.Transactions.Commands;

public class CreateOrderHandlerTests {
  private readonly IRepository<Auction> _auctionRepository = Substitute.For<IRepository<Auction>>();
  private readonly IOrderReadRepository _orderReadRepository = Substitute.For<IOrderReadRepository>();
  private readonly IRepository<Order> _orderRepository = Substitute.For<IRepository<Order>>();
  private readonly IReadRepository<CatalogItem, CatalogItemDto> _catalogItemReadRepo =
    Substitute.For<IReadRepository<CatalogItem, CatalogItemDto>>();

  private readonly CreateOrderHandler _sut;

  public CreateOrderHandlerTests() {
    _sut = new CreateOrderHandler(_auctionRepository, _orderReadRepository, _orderRepository, _catalogItemReadRepo);
  }

  [Fact]
  public async Task Handle_Should_ThrowWorkflowException_When_AuctionNotFound() {
    var request = new CreateOrderCommand(Guid.NewGuid(), "Winner", "winner@example.com", new CreateOrderRequest(Guid.NewGuid(), UseCaseTestData.CreateAddress()));

    _auctionRepository.GetByIdAsync(request.Data.AuctionId, CancellationToken.None).Returns((Auction?)null);

    var act = async () => await _sut.Handle(request, CancellationToken.None);

    var exception = await act.Should().ThrowAsync<WorkflowException>();
    exception.Which.StatusCode.Should().Be(404);
    exception.Which.Message.Should().Be("Không tìm thấy thông tin đấu giá");
  }

  [Fact]
  public async Task Handle_Should_ThrowWorkflowException_When_AuctionHasNoWinner() {
    var auction = new AuctionBuilder().Build();
    var request = new CreateOrderCommand(Guid.NewGuid(), "Winner", "winner@example.com", new CreateOrderRequest(auction.Id, UseCaseTestData.CreateAddress()));

    _auctionRepository.GetByIdAsync(request.Data.AuctionId, CancellationToken.None).Returns(auction);

    var act = async () => await _sut.Handle(request, CancellationToken.None);

    var exception = await act.Should().ThrowAsync<WorkflowException>();
    exception.Which.Message.Should().Be("Đấu giá chưa kết thúc hoặc không có người chiến thắng");
  }

  [Fact]
  public async Task Handle_Should_ThrowWorkflowException_When_RequestUserIsNotWinningBidId() {
    var auction = CreateEndedSoldAuction();
    var request = new CreateOrderCommand(Guid.NewGuid(), "Winner", "winner@example.com", new CreateOrderRequest(auction.Id, UseCaseTestData.CreateAddress()));

    _auctionRepository.GetByIdAsync(request.Data.AuctionId, CancellationToken.None).Returns(auction);

    var act = async () => await _sut.Handle(request, CancellationToken.None);

    var exception = await act.Should().ThrowAsync<WorkflowException>();
    exception.Which.StatusCode.Should().Be(403);
    exception.Which.Message.Should().Be("Bạn không phải là người chiến thắng trong phiên đấu giá này");
  }

  [Fact]
  public async Task Handle_Should_ThrowWorkflowException_When_CatalogItemNotFound() {
    var auction = CreateEndedSoldAuction();
    var request = new CreateOrderCommand(auction.WinningBidId!.Value, "Winner", "winner@example.com", new CreateOrderRequest(auction.Id, UseCaseTestData.CreateAddress()));

    _auctionRepository.GetByIdAsync(request.Data.AuctionId, CancellationToken.None).Returns(auction);
    _catalogItemReadRepo.GetByIdAsync(auction.CatalogItemId, CancellationToken.None).Returns((CatalogItemDto?)null);

    var act = async () => await _sut.Handle(request, CancellationToken.None);

    var exception = await act.Should().ThrowAsync<WorkflowException>();
    exception.Which.StatusCode.Should().Be(404);
    exception.Which.Message.Should().Be("Không có sản phẩm");
  }

  [Fact]
  public async Task Handle_Should_ReturnExistingOrderId_When_OrderAlreadyExists() {
    var auction = CreateEndedSoldAuction();
    var request = new CreateOrderCommand(auction.WinningBidId!.Value, "Winner", "winner@example.com", new CreateOrderRequest(auction.Id, UseCaseTestData.CreateAddress()));
    var item = UseCaseTestData.CreateCatalogItemDto(id: auction.CatalogItemId, name: "Laptop");
    var existingOrder = UseCaseTestData.CreateOrderDto(id: Guid.NewGuid());

    _auctionRepository.GetByIdAsync(request.Data.AuctionId, CancellationToken.None).Returns(auction);
    _catalogItemReadRepo.GetByIdAsync(auction.CatalogItemId, CancellationToken.None).Returns(item);
    _orderReadRepository.GetByAuctionIdAsync(auction.Id, CancellationToken.None).Returns(existingOrder);

    var result = await _sut.Handle(request, CancellationToken.None);

    result.Id.Should().Be(existingOrder.Id);
    await _orderRepository.DidNotReceive().CreateAsync(Arg.Any<Order>(), CancellationToken.None);
  }

  [Fact]
  public async Task Handle_Should_CreateOrder_When_NotExists() {
    var auction = CreateEndedSoldAuction();
    var request = new CreateOrderCommand(auction.WinningBidId!.Value, "Winner", "winner@example.com", new CreateOrderRequest(auction.Id, UseCaseTestData.CreateAddress()));
    var item = UseCaseTestData.CreateCatalogItemDto(id: auction.CatalogItemId, name: "Laptop");

    _auctionRepository.GetByIdAsync(request.Data.AuctionId, CancellationToken.None).Returns(auction);
    _catalogItemReadRepo.GetByIdAsync(auction.CatalogItemId, CancellationToken.None).Returns(item);
    _orderReadRepository.GetByAuctionIdAsync(auction.Id, CancellationToken.None).Returns((OrderDto?)null);

    var result = await _sut.Handle(request, CancellationToken.None);

    result.Id.Should().NotBeEmpty();
    await _orderRepository.Received(1).CreateAsync(
      Arg.Is<Order>(x =>
        x.Id == result.Id &&
        x.BidderId == request.UserId &&
        x.AuctionId == auction.Id &&
        x.CatalogId == auction.CatalogItemId &&
        x.CatalogName == item.Name
      ),
      CancellationToken.None
    );
  }

  private static Auction CreateEndedSoldAuction() {
    var auction = new AuctionBuilder().WithPrices(1000, 100, 1000).Build();
    auction.Start();
    auction.PlaceBid(Guid.NewGuid(), "Winner", 1000);
    auction.End();
    auction.Status.Should().Be(AuctionStatus.EndedSold);
    return auction;
  }
}
