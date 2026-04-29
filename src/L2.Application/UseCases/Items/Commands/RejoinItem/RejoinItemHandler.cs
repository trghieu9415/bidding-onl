using L1.Core.Domain.Catalog.Entities;
using L2.Application.Exceptions;
using L2.Application.Repositories;
using MediatR;

namespace L2.Application.UseCases.Items.Commands.RejoinItem;

public class RejoinItemHandler(
  IRepository<CatalogItem> itemRepository
) : IRequestHandler<RejoinItemCommand, bool> {
  public async Task<bool> Handle(RejoinItemCommand request, CancellationToken ct) {
    var item = await itemRepository.GetByIdAsync(request.Id, ct);
    if (item == null || request.UserId != item.OwnerId) {
      throw new WorkflowException("Không tìm thấy sản phẩm");
    }

    item.Rejoin();
    await itemRepository.UpdateAsync(item, ct);
    return true;
  }
}
