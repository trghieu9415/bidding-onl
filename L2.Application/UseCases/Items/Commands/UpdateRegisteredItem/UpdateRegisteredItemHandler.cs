using L1.Core.Domain.Catalog.Entities;
using L1.Core.Domain.Catalog.Enums;
using L2.Application.Exceptions;
using L2.Application.Ports.Security;
using L2.Application.Repositories;
using MediatR;

namespace L2.Application.UseCases.Items.Commands.UpdateRegisteredItem;

public class UpdateRegisteredItemHandler(
  IRepository<CatalogItem> repository,
  ICurrentUser currentUser
) : IRequestHandler<UpdateRegisteredItemCommand, bool> {
  public async Task<bool> Handle(UpdateRegisteredItemCommand request, CancellationToken ct) {
    var item = await repository.GetByIdAsync(request.Id, ct)
               ?? throw new WorkflowException("Sản phẩm không tồn tại", 404);

    if (item.OwnerId != currentUser.Id) {
      throw new WorkflowException("Bạn không có quyền chỉnh sửa sản phẩm này", 403);
    }

    if (item.Status != ItemStatus.Pending && item.Status != ItemStatus.Rejected) {
      throw new WorkflowException("Không thể sửa sản phẩm đã được duyệt hoặc đã bán");
    }

    var data = request.Data;

    item.Update(data.Name, data.Description);

    if (data.StartingPrice.HasValue) {
      item.SetStartingPrice(data.StartingPrice.Value);
    }

    if (data.Condition.HasValue) {
      item.SetCondition(data.Condition.Value);
    }

    if (data.CategoryIds != null) {
      item.SyncCategories(data.CategoryIds);
    }

    if (data.MainImageUrl != null || data.SubImageUrls != null) {
      item.SetImageUrls(data.MainImageUrl, data.SubImageUrls ?? []);
    }

    await repository.UpdateAsync(item, ct);
    return true;
  }
}
