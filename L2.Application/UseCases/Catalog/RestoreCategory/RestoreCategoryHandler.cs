using L1.Core.Domain.Catalog.Entities;
using L2.Application.Repositories;
using MediatR;

namespace L2.Application.UseCases.Catalog.RestoreCategory;

public class RestoreCategoryHandler(IRepository<Category> repository) : IRequestHandler<RestoreCategoryCommand, Unit> {
  public async Task<Unit> Handle(RestoreCategoryCommand request, CancellationToken ct) {
    await repository.RestoreAsync(request.Id, ct);
    return Unit.Value;
  }
}
