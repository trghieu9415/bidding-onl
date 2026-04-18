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
      request.Filter,
      ct: ct
    );

    var meta = Meta.Create(request.Filter.Page, request.Filter.PerPage, total);
    return new GetRegisteredItemsResult(entities, meta);
  }
}
