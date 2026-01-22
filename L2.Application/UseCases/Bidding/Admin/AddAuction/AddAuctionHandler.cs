using L1.Core.Domain.Bidding.Entities;
using L2.Application.Ports.Repository;
using MediatR;

namespace L2.Application.UseCases.Bidding.Admin.AddAuction;

public class AddAuctionHandler(IRepository<Auction> repository)
  : IRequestHandler<AddAuctionCommand, Guid> {
  public async Task<Guid> Handle(AddAuctionCommand request, CancellationToken ct) {
    var auction = Auction.Create(request.CatalogItemId, request.StepPrice, request.ReservePrice);
    return await repository.CreateAsync(auction, ct);
  }
}