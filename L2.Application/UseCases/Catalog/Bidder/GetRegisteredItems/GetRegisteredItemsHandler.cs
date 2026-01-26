using AutoMapper;
using L1.Core.Domain.Catalog.Entities;
using L2.Application.DTOs;
using L2.Application.Models;
using L2.Application.Ports.Repositories;
using L2.Application.Ports.Security;
using MediatR;

namespace L2.Application.UseCases.Catalog.Bidder.GetRegisteredItems;

public class GetRegisteredItemsHandler(
  IReadRepository<CatalogItem> readRepository,
  ICurrentUser currentUser,
  IMapper mapper
) : IRequestHandler<GetRegisteredItemsQuery, GetRegisteredItemsResult> {
  public async Task<GetRegisteredItemsResult> Handle(GetRegisteredItemsQuery request, CancellationToken ct) {
    var (total, entities) = await readRepository.GetAsync(
      request.SieveModel,
      x => x.OwnerId == currentUser.User.Id,
      ct: ct
    );

    var dtos = mapper.Map<List<CatalogItemDto>>(entities);
    var meta = Meta.Create(request.SieveModel.Page ?? 1, request.SieveModel.PageSize ?? 10, total);

    return new GetRegisteredItemsResult(dtos, meta);
  }
}
