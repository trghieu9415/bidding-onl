using L1.Core.Domain.Catalog.Entities;
using L2.Application.DTOs;
using L2.Application.Models;
using L2.Application.Repositories;
using MediatR;

namespace L2.Application.UseCases.Catalog.GetRemovedCategories;

public class GetRemovedCategoriesHandler(
  IReadRepository<Category, CategoryDto> readRepository
) : IRequestHandler<GetRemovedCategoriesQuery, GetRemovedCategoriesResult> {
  public async Task<GetRemovedCategoriesResult> Handle(GetRemovedCategoriesQuery request, CancellationToken ct) {
    var (total, entities) = await readRepository.GetDeletedAsync(sieveModel: request.SieveModel, ct: ct);
    var meta = Meta.Create(request.SieveModel.Page ?? 1, request.SieveModel.PageSize ?? 10, total);
    return new GetRemovedCategoriesResult(entities, meta);
  }
}
