using L1.Core.Domain.Catalog.Entities;
using L2.Application.Repositories;
using MediatR;

namespace L2.Application.UseCases.Items.Commands.RegisterItem;

public class RegisterItemHandler(
  IRepository<CatalogItem> repository
) : IRequestHandler<RegisterItemCommand, Guid> {
  public async Task<Guid> Handle(RegisterItemCommand request, CancellationToken ct) {
    var data = request.Data;
    var item = CatalogItem.Create(request.UserId, data.Name, data.Description);

    item.SetStartingPrice(data.StartingPrice)
      .SetCondition(data.Condition)
      .SyncCategories(data.CategoryIds)
      .SetImageUrls(data.MainImageUrl, data.SubImageUrls);

    return await repository.CreateAsync(item, ct);
  }
}
