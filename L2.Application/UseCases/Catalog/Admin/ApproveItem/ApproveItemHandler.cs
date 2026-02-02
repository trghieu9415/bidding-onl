using L1.Core.Domain.Catalog.Entities;
using L2.Application.Exceptions;
using L2.Application.Ports.Repositories;
using MediatR;

namespace L2.Application.UseCases.Catalog.Admin.ApproveItem;

public class ApproveItemHandler(
  IRepository<CatalogItem> repository
) : IRequestHandler<ApproveItemCommand, Unit> {
  public async Task<Unit> Handle(ApproveItemCommand request, CancellationToken ct) {
    var item = await repository.GetByIdAsync(request.Id, ct)
               ?? throw new AppException("Sản phẩm không tồn tại", 404);

    item.Approve();
    await repository.UpdateAsync(item, ct);
    return Unit.Value;
  }
}
