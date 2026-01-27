using AutoMapper;
using L1.Core.Domain.Bidding.Entities;
using L2.Application.DTOs;
using L2.Application.Exceptions;
using L2.Application.Ports.Repositories;
using MediatR;

namespace L2.Application.UseCases.Bidding.Admin.GetSession;

public class GetSessionHandler(
  IReadRepository<AuctionSession> readRepository,
  IMapper mapper
) : IRequestHandler<GetSessionQuery, GetSessionResult> {
  public async Task<GetSessionResult> Handle(GetSessionQuery request, CancellationToken ct) {
    var session = await readRepository.GetByIdAsync(request.Id, ct)
                  ?? throw new AppException("Phiên đấu giá không tồn tại hoặc đã bị xóa", 404);

    var dto = mapper.Map<AuctionSessionDto>(session);

    return new GetSessionResult(dto);
  }
}
