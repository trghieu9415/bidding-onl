using AutoMapper;
using L1.Core.Domain.Catalog.Entities;
using L2.Application.DTOs;
using L2.Application.Models;
using L2.Application.Ports.Repositories;
using MediatR;

namespace L2.Application.UseCases.Catalog.Admin.GetRemovedCategories;

public class GetRemovedCategoriesHandler(
  IReadRepository<Category> readRepository,
  IMapper mapper
) : IRequestHandler<GetRemovedCategoriesQuery, GetRemovedCategoriesResult> {
  public async Task<GetRemovedCategoriesResult> Handle(GetRemovedCategoriesQuery request, CancellationToken ct) {
    var (total, entities) = await readRepository.GetDeletedAsync(request.SieveModel, ct: ct);

    var dtos = mapper.Map<List<CategoryDto>>(entities);
    var meta = Meta.Create(request.SieveModel.Page ?? 1, request.SieveModel.PageSize ?? 10, total);

    return new GetRemovedCategoriesResult(dtos, meta);
  }
}
