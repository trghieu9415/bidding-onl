using L1.Core.Domain.Catalog.Entities;
using L2.Application.DTOs;
using L2.Application.Models;
using L2.Application.Repositories;
using MediatR;

namespace L2.Application.UseCases.Items.Queries.GetRegisteredItems;

public class GetRegisteredItemsHandler(
  IReadRepository<CatalogItem, CatalogItemDto> readRepository
) : IRequestHandler<GetRegisteredItemsQuery, GetRegisteredItemsResult> {
  public async Task<GetRegisteredItemsResult> Handle(GetRegisteredItemsQuery request, CancellationToken ct) {
    var (total, entities) = await readRepository.GetAsync(
      x => x.OwnerId == request.UserId,
      request.SieveModel,
      ct: ct
    );

    var meta = Meta.Create(request.SieveModel.Page ?? 1, request.SieveModel.PageSize ?? 10, total);
    return new GetRegisteredItemsResult(entities, meta);
  }
}
