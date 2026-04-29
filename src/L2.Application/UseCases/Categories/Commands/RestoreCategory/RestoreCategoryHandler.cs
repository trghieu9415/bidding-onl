using L1.Core.Domain.Catalog.Entities;
using L2.Application.Repositories;
using MediatR;

namespace L2.Application.UseCases.Categories.Commands.RestoreCategory;

public class RestoreCategoryHandler(IRepository<Category> repository) : IRequestHandler<RestoreCategoryCommand, bool> {
  public async Task<bool> Handle(RestoreCategoryCommand request, CancellationToken ct) {
    await repository.RestoreAsync(request.Id, ct);
    return true;
  }
}
