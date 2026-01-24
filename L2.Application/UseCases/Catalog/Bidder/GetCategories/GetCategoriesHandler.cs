using AutoMapper;
using L1.Core.Domain.Catalog.Entities;
using L2.Application.DTOs;
using L2.Application.Models;
using L2.Application.Ports.Repository;
using MediatR;

namespace L2.Application.UseCases.Catalog.Bidder.GetCategories;

public class GetCategoriesHandler(IReadRepository<Category> readRepository, IMapper mapper)
  : IRequestHandler<GetCategoriesQuery, GetCategoriesResult> {
  public async Task<GetCategoriesResult> Handle(GetCategoriesQuery request, CancellationToken ct) {
    var (total, entities) = await readRepository.GetAsync(request.SieveModel, ct: ct);
    var dtos = mapper.Map<List<CategoryDto>>(entities);
    var meta = Meta.Create(request.SieveModel.Page ?? 1, request.SieveModel.PageSize ?? 10, total);
    return new GetCategoriesResult(dtos, meta);
  }
}
