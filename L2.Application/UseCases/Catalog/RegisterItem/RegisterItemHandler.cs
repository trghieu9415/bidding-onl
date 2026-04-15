using L1.Core.Domain.Catalog.Entities;
using L2.Application.Ports.Security;
using L2.Application.Repositories;
using MediatR;

namespace L2.Application.UseCases.Catalog.RegisterItem;

public class RegisterItemHandler(
  IRepository<CatalogItem> repository,
  ICurrentUser currentUser
) : IRequestHandler<RegisterItemCommand, Guid> {
  public async Task<Guid> Handle(RegisterItemCommand request, CancellationToken ct) {
    var item = CatalogItem.Create(currentUser.Id, request.Name, request.Description);

    item.SetStartingPrice(request.StartingPrice)
      .SetCondition(request.Condition)
      .SyncCategories(request.CategoryIds)
      .SetImageUrls(request.MainImageUrl, request.SubImageUrls);

    return await repository.CreateAsync(item, ct);
  }
}
