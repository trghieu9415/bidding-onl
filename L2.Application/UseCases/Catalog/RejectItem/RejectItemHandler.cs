using L1.Core.Domain.Catalog.Entities;
using L2.Application.Exceptions;
using L2.Application.Repositories;
using MediatR;

namespace L2.Application.UseCases.Catalog.RejectItem;

public class RejectItemHandler(IRepository<CatalogItem> repository) : IRequestHandler<RejectItemCommand, Unit> {
  public async Task<Unit> Handle(RejectItemCommand request, CancellationToken ct) {
    var item = await repository.GetByIdAsync(request.Id, ct)
               ?? throw new WorkflowException("Sản phẩm không tồn tại", 404);

    item.Reject(request.Reason);
    await repository.UpdateAsync(item, ct);
    return Unit.Value;
  }
}
