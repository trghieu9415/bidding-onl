using L1.Core.Domain.Transaction.Entities;
using L2.Application.DTOs;
using L2.Application.Models;
using L2.Application.Ports.Security;
using L2.Application.Repositories;
using MediatR;

namespace L2.Application.UseCases.Transaction.GetOrderHistory;

public class GetOrderHistoryHandler(
  IReadRepository<Order, OrderDto> readRepository,
  ICurrentUser currentUser
) : IRequestHandler<GetOrderHistoryQuery, GetOrderHistoryResult> {
  public async Task<GetOrderHistoryResult> Handle(GetOrderHistoryQuery request, CancellationToken ct) {
    var (total, entities) = await readRepository.GetAsync(
      x => x.BidderId == currentUser.Id,
      request.SieveModel,
      ct: ct
    );

    var meta = Meta.Create(request.SieveModel.Page ?? 1, request.SieveModel.PageSize ?? 10, total);
    return new GetOrderHistoryResult(entities, meta);
  }
}
