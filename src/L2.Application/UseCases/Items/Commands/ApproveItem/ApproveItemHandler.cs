using L1.Core.Domain.Catalog.Entities;
using L2.Application.Exceptions;
using L2.Application.Repositories;
using MediatR;

namespace L2.Application.UseCases.Items.Commands.ApproveItem;

public class ApproveItemHandler(
  IRepository<CatalogItem> repository
) : IRequestHandler<ApproveItemCommand, bool> {
  public async Task<bool> Handle(ApproveItemCommand request, CancellationToken ct) {
    var item = await repository.GetByIdAsync(request.Id, ct)
               ?? throw new WorkflowException("Sản phẩm không tồn tại", 404);

    item.Approve();
    await repository.UpdateAsync(item, ct);
    return true;
  }
}
