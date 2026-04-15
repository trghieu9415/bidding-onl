using L1.Core.Domain.Catalog.Entities;
using L2.Application.DTOs;
using L2.Application.Models;
using L2.Application.Repositories;
using MediatR;

namespace L2.Application.UseCases.Items.Queries.GetItems;

public class GetItemsHandler(IReadRepository<CatalogItem, CatalogItemDto> readRepository)
  : IRequestHandler<GetItemsQuery, GetItemsResult> {
  public async Task<GetItemsResult> Handle(GetItemsQuery request, CancellationToken ct) {
    var (total, entities) = await readRepository.GetAsync(sieveModel: request.SieveModel, ct: ct);
    var meta = Meta.Create(request.SieveModel.Page ?? 1, request.SieveModel.PageSize ?? 10, total);
    return new GetItemsResult(entities, meta);
  }
}
