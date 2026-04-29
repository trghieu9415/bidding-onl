using L1.Core.Domain.Catalog.Entities;
using L2.Application.DTOs;
using L2.Application.Models;
using L2.Application.Repositories;
using MediatR;

namespace L2.Application.UseCases.Categories.Queries.GetRemovedCategories;

public class GetRemovedCategoriesHandler(
  IReadRepository<Category, CategoryDto> readRepository
) : IRequestHandler<GetRemovedCategoriesQuery, GetRemovedCategoriesResult> {
  public async Task<GetRemovedCategoriesResult> Handle(GetRemovedCategoriesQuery request, CancellationToken ct) {
    var (total, entities) = await readRepository.GetDeletedAsync(filter: request.Filter, ct: ct);
    var meta = Meta.Create(request.Filter.Page, request.Filter.PerPage, total);
    return new GetRemovedCategoriesResult(entities, meta);
  }
}
