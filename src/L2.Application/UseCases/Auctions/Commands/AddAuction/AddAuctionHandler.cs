using L1.Core.Domain.Bidding.Entities;
using L1.Core.Domain.Catalog.Entities;
using L1.Core.Domain.Catalog.Enums;
using L2.Application.DTOs;
using L2.Application.Exceptions;
using L2.Application.Repositories;
using MediatR;

namespace L2.Application.UseCases.Auctions.Commands.AddAuction;

public class AddAuctionHandler(
  IRepository<Auction> auctionRepository,
  IReadRepository<CatalogItem, CatalogItemDto> itemRepository
) : IRequestHandler<AddAuctionCommand, Guid> {
  public async Task<Guid> Handle(AddAuctionCommand request, CancellationToken ct) {
    var item = await itemRepository.GetByIdAsync(request.CatalogItemId, ct)
               ?? throw new WorkflowException($"Không tìm thấy vật phẩm - Id: {request.CatalogItemId}", 404);

    if (item.Status != ItemStatus.Approval) {
      throw new WorkflowException($"Sản phẩm chưa được phê duyệt để sẵn sàng để đấu giá - Id: {item.Id}");
    }

    var auction = Auction.Create(
      item.Id,
      request.SessionId,
      item.StartingPrice,
      request.StepPrice,
      request.ReservePrice
    ).SetOwnerId(item.OwnerId);
    return await auctionRepository.CreateAsync(auction, ct);
  }
}
