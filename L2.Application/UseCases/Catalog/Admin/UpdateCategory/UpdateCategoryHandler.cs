using L1.Core.Domain.Catalog.Entities;
using L2.Application.Exceptions;
using L2.Application.Repositories;
using MediatR;

namespace L2.Application.UseCases.Catalog.Admin.UpdateCategory;

public class UpdateCategoryHandler(IRepository<Category> repository) : IRequestHandler<UpdateCategoryCommand, Unit> {
  public async Task<Unit> Handle(UpdateCategoryCommand request, CancellationToken ct) {
    var category = await repository.GetByIdAsync(request.Id, ct)
                   ?? throw new WorkflowException("Không tìm thấy danh mục", 404);

    category.Update(request.Name, request.ParentId);
    await repository.UpdateAsync(category, ct);
    return Unit.Value;
  }
}
