using L1.Core.Domain.Catalog.Entities;
using L2.Application.Exceptions;
using L2.Application.Repositories;
using MediatR;

namespace L2.Application.UseCases.Items.Commands.RejectItem;

public class RejectItemHandler(IRepository<CatalogItem> repository)
  : IRequestHandler<RejectItemCommand, bool> {
  public async Task<bool> Handle(RejectItemCommand request, CancellationToken ct) {
    var item = await repository.GetByIdAsync(request.Id, ct)
               ?? throw new WorkflowException("Sản phẩm không tồn tại", 404);

    item.Reject(request.Data.Reason);
    await repository.UpdateAsync(item, ct);
    return true;
  }
}
