using L1.Core.Domain.Catalog.Entities;
using L2.Application.DTOs;
using L2.Application.Models;
using L2.Application.Repositories;
using MediatR;

namespace L2.Application.UseCases.Categories.Queries.GetCategories;

public class GetCategoriesHandler(
  IReadRepository<Category, CategoryDto> readRepository
)
  : IRequestHandler<GetCategoriesQuery, GetCategoriesResult> {
  public async Task<GetCategoriesResult> Handle(GetCategoriesQuery request, CancellationToken ct) {
    var (total, entities) = await readRepository.GetAsync(filter: request.Filter, ct: ct);
    var meta = Meta.Create(request.Filter.Page, request.Filter.PerPage, total);
    return new GetCategoriesResult(entities, meta);
  }
}
