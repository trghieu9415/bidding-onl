using L1.Core.Domain.Catalog.Entities;
using L2.Application.Repositories;
using MediatR;

namespace L2.Application.UseCases.Items.Commands.RestoreItem;

public class RestoreItemHandler(IRepository<CatalogItem> repository) : IRequestHandler<RestoreItemCommand, bool> {
  public async Task<bool> Handle(RestoreItemCommand request, CancellationToken ct) {
    await repository.RestoreAsync(request.Id, ct);
    return true;
  }
}
