using System.ComponentModel.DataAnnotations;
using L1.Core.Base.Entity;
using L1.Core.Domain.Catalog.Enums;
using L1.Core.Domain.Catalog.Events;
using L1.Core.Domain.Catalog.ValueObjects;
using L1.Core.Exceptions;

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
    var item = new CatalogItem {
      OwnerId = ownerId,
      Name = name,
      Description = description
    };
    item.AddDomainEvent(new ItemRegisteredEvent(item.Id, ownerId, name));
    return item;
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

  public void Reject(string reason = "") {
    if (Status != ItemStatus.Pending) {
      throw new DomainException("Chỉ có thể từ chối sản phẩm đang chờ duyệt.");
    }

    Status = ItemStatus.Rejected;
    AddDomainEvent(new ItemRejectedEvent(Id, OwnerId, reason));
  }


  public void Approve() {
    if (Status != ItemStatus.Pending) {
      throw new DomainException("Chỉ có thể duyệt sản phẩm đang chờ duyệt.");
    }

    Status = ItemStatus.Approval;
    AddDomainEvent(new ItemApprovedEvent(Id, OwnerId));
  }

  public void Sell(bool isSold) {
    Status = isSold ? ItemStatus.Sold : ItemStatus.Unsold;
  }
}
