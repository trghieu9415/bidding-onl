using L1.Core.Domain.Catalog.Entities;
using L2.Application.Repositories;
using MediatR;

namespace L2.Application.UseCases.Catalog.AddCategory;

public class AddCategoryHandler(IRepository<Category> repository) : IRequestHandler<AddCategoryCommand, Guid> {
  public async Task<Guid> Handle(AddCategoryCommand request, CancellationToken ct) {
    var category = Category.Create(request.Name, request.ParentId);
    return await repository.CreateAsync(category, ct);
  }
}
