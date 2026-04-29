using L1.Core.Domain.Catalog.Entities;
using L2.Application.Exceptions;
using L2.Application.Repositories;
using MediatR;

namespace L2.Application.UseCases.Items.Commands.RevokeItem;

public class RevokeItemHandler(
  IRepository<CatalogItem> itemRepository
) : IRequestHandler<RevokeItemCommand, bool> {
  public async Task<bool> Handle(RevokeItemCommand request, CancellationToken ct) {
    var item = await itemRepository.GetByIdAsync(request.Id, ct);
    if (item == null || request.UserId != item.OwnerId) {
      throw new WorkflowException("Không tìm thấy sản phẩm");
    }

    item.Revoke();
    await itemRepository.UpdateAsync(item, ct);
    return true;
  }
}
