using L1.Core.Domain.Catalog.Entities;
using L1.Core.Domain.Catalog.Enums;
using L2.Application.Exceptions;
using L2.Application.Ports.Repository;
using L2.Application.Ports.Security;
using MediatR;

namespace L2.Application.UseCases.Catalog.Bidder.UpdateRegisteredItem;

public class UpdateRegisteredItemHandler(
  IRepository<CatalogItem> repository,
  ICurrentUser currentUser
) : IRequestHandler<UpdateRegisteredItemCommand, Unit> {
  public async Task<Unit> Handle(UpdateRegisteredItemCommand request, CancellationToken ct) {
    var item = await repository.GetByIdAsync(request.Id, ct)
               ?? throw new AppException("Sản phẩm không tồn tại", 404);

    if (item.OwnerId != currentUser.User.Id) {
      throw new AppException("Bạn không có quyền chỉnh sửa sản phẩm này", 403);
    }

    if (item.Status != ItemStatus.Pending && item.Status != ItemStatus.Rejected) {
      throw new AppException("Không thể sửa sản phẩm đã được duyệt hoặc đã bán");
    }

    item.Update(request.Name, request.Description);

    if (request.StartingPrice.HasValue) {
      item.SetStartingPrice(request.StartingPrice.Value);
    }

    if (request.Condition.HasValue) {
      item.SetCondition(request.Condition.Value);
    }

    if (request.CategoryIds != null) {
      item.SyncCategories(request.CategoryIds);
    }

    if (request.MainImageUrl != null || request.SubImageUrls != null) {
      item.SetImageUrls(request.MainImageUrl, request.SubImageUrls ?? []);
    }

    await repository.UpdateAsync(item, ct);
    return Unit.Value;
  }
}