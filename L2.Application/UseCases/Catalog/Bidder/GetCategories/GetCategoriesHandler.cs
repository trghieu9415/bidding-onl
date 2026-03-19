using L1.Core.Domain.Catalog.Entities;
using L2.Application.DTOs;
using L2.Application.Models;
using L2.Application.Repositories;
using MediatR;

namespace L2.Application.UseCases.Catalog.Bidder.GetCategories;

public class GetCategoriesHandler(IReadRepository<Category, CategoryDto> readRepository)
  : IRequestHandler<GetCategoriesQuery, GetCategoriesResult> {
  public async Task<GetCategoriesResult> Handle(GetCategoriesQuery request, CancellationToken ct) {
    var (total, entities) = await readRepository.GetAsync(sieveModel: request.SieveModel, ct: ct);
    var meta = Meta.Create(request.SieveModel.Page ?? 1, request.SieveModel.PageSize ?? 10, total);
    return new GetCategoriesResult(entities, meta);
  }
}
