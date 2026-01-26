using AutoMapper;
using L1.Core.Domain.Bidding.Entities;
using L2.Application.DTOs;
using L2.Application.Exceptions;
using L2.Application.Ports.Repositories;
using MediatR;

namespace L2.Application.UseCases.Bidding.Admin.GetAuction;

public class GetAuctionHandler(IReadRepository<Auction> readRepository, IMapper mapper)
  : IRequestHandler<GetAuctionQuery, GetAuctionResult> {
  public async Task<GetAuctionResult> Handle(GetAuctionQuery request, CancellationToken ct) {
    var auction = await readRepository.GetByIdAsync(request.Id, ct)
                  ?? throw new AppException("Đấu giá không tồn tại", 404);

    return new GetAuctionResult(mapper.Map<AuctionDto>(auction));
  }
}
