using L1.Core.Domain.Catalog.Entities;
using L2.Application.DTOs;
using L2.Application.Exceptions;
using L2.Application.Repositories;
using MediatR;

namespace L2.Application.UseCases.Catalog.GetCategory;

public class GetCategoryHandler(
  IReadRepository<Category, CategoryDto> readRepository
) : IRequestHandler<GetCategoryQuery, GetCategoryResult> {
  public async Task<GetCategoryResult> Handle(GetCategoryQuery request, CancellationToken ct) {
    var cat = await readRepository.GetByIdAsync(request.Id, ct)
              ?? throw new WorkflowException("Danh mục không tồn tại", 404);

    return new GetCategoryResult(cat);
  }
}
