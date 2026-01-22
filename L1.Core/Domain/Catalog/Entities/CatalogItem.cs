using System.ComponentModel.DataAnnotations;
using L1.Core.Base.Entity;
using L1.Core.Base.Exception;
using L1.Core.Domain.Catalog.Enums;
using L1.Core.Domain.Catalog.ValueObjects;

namespace L1.Core.Domain.Catalog.Entities;

public class CatalogItem : AggregateRoot {
  private readonly List<Guid> _categoryIds = [];
  private CatalogItem() {}

  public Guid OwnerId { get; private set; }
  public IReadOnlyCollection<Guid> CategoryIds => _categoryIds.AsReadOnly();
  [Required] public string Name { get; private set; } = null!;
  [Required] public string Description { get; private set; } = null!;
  public ItemStatus Status { get; private set; } = ItemStatus.Pending;
  public ItemCondition? Condition { get; private set; }
  public decimal StartingPrice { get; private set; }
  public ImageGallery Images { get; private set; } = new(null, []);

  public static CatalogItem Create(Guid ownerId, string name, string description) {
    return new CatalogItem {
      OwnerId = ownerId,
      Name = name,
      Description = description
    };
  }

  public CatalogItem Update(string? name, string? description) {
    Name = name ?? Name;
    Description = description ?? Description;
    return this;
  }

  public CatalogItem SetCondition(ItemCondition condition) {
    Condition = condition;
    return this;
  }

  public CatalogItem SetStartingPrice(decimal startingPrice) {
    StartingPrice = startingPrice;
    return this;
  }

  public CatalogItem SyncCategories(ICollection<Guid> categoryIds) {
    _categoryIds.Clear();
    _categoryIds.AddRange(categoryIds);
    return this;
  }

  public CatalogItem SetImageUrls(string? mainImageUrl, ICollection<string> subImageUrls) {
    Images = new ImageGallery(
      mainImageUrl ?? Images.MainImageUrl,
      subImageUrls
    );
    return this;
  }

  public void Reject() {
    if (Status != ItemStatus.Pending) {
      throw new DomainException("Chỉ có thể từ chối sản phẩm đang chờ duyệt.");
    }

    Status = ItemStatus.Rejected;
  }


  public void Approve() {
    if (Status != ItemStatus.Pending) {
      throw new DomainException("Chỉ có thể duyệt sản phẩm đang chờ duyệt.");
    }

    Status = ItemStatus.Approval;
  }

  public void Sell(bool isSold) {
    if (Status != ItemStatus.Approval) {
      throw new DomainException("Chỉ có thể chuyển trạng thái sản phẩm khi sản phẩm được duyệt.");
    }

    Status = isSold ? ItemStatus.Sold : ItemStatus.Unsold;
  }
}