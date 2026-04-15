using L1.Core.Domain.Transaction.Entities;
using L2.Application.DTOs;
using L2.Application.Models;
using L2.Application.Repositories;
using MediatR;

namespace L2.Application.UseCases.Transactions.Queries.GetOrders;

public class GetOrdersHandler(IReadRepository<Order, OrderDto> readRepository)
  : IRequestHandler<GetOrdersQuery, GetOrdersResult> {
  public async Task<GetOrdersResult> Handle(GetOrdersQuery request, CancellationToken ct) {
    var (total, entities) = await readRepository.GetAsync(sieveModel: request.SieveModel, ct: ct);
    var meta = Meta.Create(request.SieveModel.Page ?? 1, request.SieveModel.PageSize ?? 10, total);
    return new GetOrdersResult(entities, meta);
  }
}
