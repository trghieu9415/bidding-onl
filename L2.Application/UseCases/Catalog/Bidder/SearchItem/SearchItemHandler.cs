using AutoMapper;
using L1.Core.Domain.Catalog.Entities;
using L1.Core.Domain.Catalog.Enums;
using L2.Application.DTOs;
using L2.Application.Models;
using L2.Application.Ports.Repositories;
using MediatR;

namespace L2.Application.UseCases.Catalog.Bidder.SearchItem;

public class SearchItemHandler(IReadRepository<CatalogItem> readRepo, IMapper mapper)
  : IRequestHandler<SearchItemQuery, SearchItemResult> {
  public async Task<SearchItemResult> Handle(SearchItemQuery request, CancellationToken ct) {
    var (total, entities) = await readRepo.GetAsync(
      request.SieveModel,
      x => x.Status == ItemStatus.Approval,
      ct: ct
    );

    var dtos = mapper.Map<List<CatalogItemDto>>(entities);
    var meta = Meta.Create(request.SieveModel.Page ?? 1, request.SieveModel.PageSize ?? 10, total);

    return new SearchItemResult(dtos, meta);
  }
}
