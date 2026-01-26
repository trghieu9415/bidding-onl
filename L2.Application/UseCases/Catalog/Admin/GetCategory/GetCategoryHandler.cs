using AutoMapper;
using L1.Core.Domain.Catalog.Entities;
using L2.Application.DTOs;
using L2.Application.Exceptions;
using L2.Application.Ports.Repositories;
using MediatR;

namespace L2.Application.UseCases.Catalog.Admin.GetCategory;

public class GetCategoryHandler(
  IReadRepository<Category> readRepository,
  IMapper mapper
)
  : IRequestHandler<GetCategoryQuery, GetCategoryResult> {
  public async Task<GetCategoryResult> Handle(GetCategoryQuery request, CancellationToken ct) {
    var cat = await readRepository.GetByIdAsync(request.Id, ct)
              ?? throw new AppException("Danh mục không tồn tại", 404);

    var dto = mapper.Map<CategoryDto>(cat);
    return new GetCategoryResult(dto);
  }
}
