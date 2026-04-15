using L1.Core.Domain.Catalog.Entities;
using L2.Application.Repositories;
using MediatR;

namespace L2.Application.UseCases.Categories.Commands.RemoveCategory;

public class RemoveCategoryHandler(IRepository<Category> repository) : IRequestHandler<RemoveCategoryCommand, Unit> {
  public async Task<Unit> Handle(RemoveCategoryCommand request, CancellationToken ct) {
    await repository.DeleteAsync(request.Id, true, ct);
    return Unit.Value;
  }
}
