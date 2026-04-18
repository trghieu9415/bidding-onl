using L1.Core.Domain.Bidding.Entities;
using L1.Core.Domain.Bidding.Enums;
using L1.Core.Domain.Catalog.Entities;
using L1.Core.Domain.Transaction.Entities;
using L2.Application.DTOs;
using L2.Application.Exceptions;
using L2.Application.Repositories;
using L2.Application.Repositories.Read;
using MediatR;

namespace L2.Application.UseCases.Transactions.Commands.CreateOrder;

public class CreateOrderHandler(
  IRepository<Auction> auctionRepository,
  IOrderReadRepository orderReadRepository,
  IRepository<Order> orderRepository,
  IReadRepository<CatalogItem, CatalogItemDto> catalogItemReadRepo
) : IRequestHandler<CreateOrderCommand, CreateOrderResult> {
  public async Task<CreateOrderResult> Handle(CreateOrderCommand request, CancellationToken ct) {
    var data = request.Data;
    var auction =
      await auctionRepository.GetByIdAsync(data.AuctionId, ct)
      ?? throw new WorkflowException("Không tìm thấy thông tin đấu giá", 404);

    if (auction.Status != AuctionStatus.EndedSold) {
      throw new WorkflowException("Đấu giá chưa kết thúc hoặc không có người chiến thắng");
    }

    if (auction.WinningBidId != request.UserId) {
      throw new WorkflowException("Bạn không phải là người chiến thắng trong phiên đấu giá này", 403);
    }

    var item =
      await catalogItemReadRepo.GetByIdAsync(auction.CatalogItemId, ct)
      ?? throw new WorkflowException("Không có sản phẩm", 404);

    var order = await orderReadRepository.GetByAuctionIdAsync(auction.Id, ct);
    if (order != null) {
      return new CreateOrderResult(order.Id);
    }

    var newOrder = Order.Create(
      request.UserId,
      request.UserFullName,
      request.UserEmail,
      auction.Id,
      auction.CatalogItemId,
      item.Name,
      item.MainImageUrl ?? string.Empty,
      data.Address
    );

    await orderRepository.CreateAsync(newOrder, ct);
    return new CreateOrderResult(newOrder.Id);
  }
}
