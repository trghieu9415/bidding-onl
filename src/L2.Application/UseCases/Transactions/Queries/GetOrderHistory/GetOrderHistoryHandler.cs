using L1.Core.Domain.Transaction.Entities;
using L2.Application.DTOs;
using L2.Application.Models;
using L2.Application.Repositories;
using MediatR;

namespace L2.Application.UseCases.Transactions.Queries.GetOrderHistory;

public class GetOrderHistoryHandler(
  IReadRepository<Order, OrderDto> readRepository
) : IRequestHandler<GetOrderHistoryQuery, GetOrderHistoryResult> {
  public async Task<GetOrderHistoryResult> Handle(GetOrderHistoryQuery request, CancellationToken ct) {
    var (total, entities) = await readRepository.GetAsync(
      x => x.BidderId == request.UserId,
      request.Filter,
      ct: ct
    );

    var meta = Meta.Create(request.Filter.Page, request.Filter.PerPage, total);
    return new GetOrderHistoryResult(entities, meta);
  }
}
