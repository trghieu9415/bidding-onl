using L1.Core.Domain.Catalog.Entities;
using L2.Application.Exceptions;
using L2.Application.Repositories;
using MediatR;

namespace L2.Application.UseCases.Categories.Commands.UpdateCategory;

public class UpdateCategoryHandler(IRepository<Category> repository) : IRequestHandler<UpdateCategoryCommand, bool> {
  public async Task<bool> Handle(UpdateCategoryCommand request, CancellationToken ct) {
    var category =
      await repository.GetByIdAsync(request.Id, ct)
      ?? throw new WorkflowException("Không tìm thấy danh mục", 404);

    var data = request.Data;

    category.Update(data.Name, data.ParentId);
    await repository.UpdateAsync(category, ct);
    return true;
  }
}
