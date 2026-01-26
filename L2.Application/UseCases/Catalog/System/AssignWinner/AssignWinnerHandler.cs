using L1.Core.Domain.Catalog.Entities;
using L2.Application.Exceptions;
using L2.Application.Ports.Repositories;
using MediatR;

namespace L2.Application.UseCases.Catalog.System.AssignWinner;

public class AssignWinnerHandler(IRepository<CatalogItem> repository)
  : IRequestHandler<AssignWinnerCommand, Unit> {
  public async Task<Unit> Handle(AssignWinnerCommand request, CancellationToken ct) {
    var item = await repository.GetByIdAsync(request.CatalogItemId, ct)
               ?? throw new AppException("Sản phẩm không tồn tại trong hệ thống", 404);

    item.Sell(request.IsSold);
    await repository.UpdateAsync(item, ct);
    return Unit.Value;
  }
}
