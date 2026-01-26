using AutoMapper;
using L1.Core.Domain.Catalog.Entities;
using L2.Application.DTOs;
using L2.Application.Models;
using L2.Application.Ports.Repositories;
using MediatR;

namespace L2.Application.UseCases.Catalog.Admin.GetItems;

public class GetItemsHandler(IReadRepository<CatalogItem> readRepository, IMapper mapper)
  : IRequestHandler<GetItemsQuery, GetItemsResult> {
  public async Task<GetItemsResult> Handle(GetItemsQuery request, CancellationToken ct) {
    var (total, entities) = await readRepository.GetAsync(request.SieveModel, ct: ct);
    var dtos = mapper.Map<List<CatalogItemDto>>(entities);
    var meta = Meta.Create(request.SieveModel.Page ?? 1, request.SieveModel.PageSize ?? 10, total);
    return new GetItemsResult(dtos, meta);
  }
}
