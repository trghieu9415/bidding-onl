using L1.Core.Domain.Catalog.Entities;
using L1.Core.Domain.Catalog.Enums;
using L1.Core.Domain.Catalog.Events;
using L1.Core.Exceptions;
using Xunit;

namespace Tests.Unit.L1.Core.Domain.Entities;

public class CatalogItemTests {
  [Fact]
  public void Create_ValidParameters_InitializesPendingItemAndRaisesRegisteredEvent() {
    var ownerId = Guid.NewGuid();

    var item = CatalogItem.Create(ownerId, "Laptop", "Gaming laptop");

    Assert.Equal(ownerId, item.OwnerId);
    Assert.Equal("Laptop", item.Name);
    Assert.Equal("Gaming laptop", item.Description);
    Assert.Equal(ItemStatus.Pending, item.Status);
    Assert.Null(item.Condition);
    Assert.Equal(0m, item.StartingPrice);
    Assert.Empty(item.CategoryIds);
    Assert.Null(item.Images.MainImageUrl);
    Assert.Empty(item.Images.SubImageUrls);

    var registeredEvent = Assert.IsType<ItemRegisteredEvent>(Assert.Single(item.DomainEvents));
    Assert.Equal(item.Id, registeredEvent.AggregateId);
    Assert.Equal(ownerId, registeredEvent.OwnerId);
    Assert.Equal("Laptop", registeredEvent.Name);
  }

  [Fact]
  public void Update_WithProvidedValues_ChangesFieldsAndReturnsSameItem() {
    var item = CatalogItem.Create(Guid.NewGuid(), "Laptop", "Gaming laptop");

    var returnedItem = item.Update("Ultrabook", "Portable laptop");

    Assert.Same(item, returnedItem);
    Assert.Equal("Ultrabook", item.Name);
    Assert.Equal("Portable laptop", item.Description);
  }

  [Fact]
  public void Update_WithNullValues_KeepsCurrentFields() {
    var item = CatalogItem.Create(Guid.NewGuid(), "Laptop", "Gaming laptop");

    item.Update(null, null);

    Assert.Equal("Laptop", item.Name);
    Assert.Equal("Gaming laptop", item.Description);
  }

  [Fact]
  public void SetCondition_SetsConditionAndReturnsSameItem() {
    var item = CatalogItem.Create(Guid.NewGuid(), "Laptop", "Gaming laptop");

    var returnedItem = item.SetCondition(ItemCondition.OpenBox);

    Assert.Same(item, returnedItem);
    Assert.Equal(ItemCondition.OpenBox, item.Condition);
  }

  [Fact]
  public void SetStartingPrice_SetsPriceAndReturnsSameItem() {
    var item = CatalogItem.Create(Guid.NewGuid(), "Laptop", "Gaming laptop");

    var returnedItem = item.SetStartingPrice(250m);

    Assert.Same(item, returnedItem);
    Assert.Equal(250m, item.StartingPrice);
  }

  [Fact]
  public void SyncCategories_ReplacesCategoryIdsAndReturnsSameItem() {
    var item = CatalogItem.Create(Guid.NewGuid(), "Laptop", "Gaming laptop");
    item.SyncCategories([Guid.NewGuid()]);
    var categoryIds = new[] { Guid.NewGuid(), Guid.NewGuid() };

    var returnedItem = item.SyncCategories(categoryIds);

    Assert.Same(item, returnedItem);
    Assert.Equal(categoryIds, item.CategoryIds);
  }

  [Fact]
  public void SetImageUrls_WithExplicitMainImage_UpdatesGallery() {
    var item = CatalogItem.Create(Guid.NewGuid(), "Laptop", "Gaming laptop");
    var imageUrls = new[] { "sub-1.png", "sub-2.png" };

    var returnedItem = item.SetImageUrls("main.png", imageUrls);

    Assert.Same(item, returnedItem);
    Assert.Equal("main.png", item.Images.MainImageUrl);
    Assert.Equal(imageUrls, item.Images.SubImageUrls);
  }

  [Fact]
  public void SetImageUrls_WhenMainImageIsNull_KeepsCurrentMainImage() {
    var item = CatalogItem.Create(Guid.NewGuid(), "Laptop", "Gaming laptop");
    item.SetImageUrls("main.png", ["sub-1.png"]);

    item.SetImageUrls(null, ["sub-2.png"]);

    Assert.Equal("main.png", item.Images.MainImageUrl);
    Assert.Equal(["sub-2.png"], item.Images.SubImageUrls);
  }

  [Fact]
  public void Reject_WhenPending_ChangesStatusAndRaisesRejectedEvent() {
    var item = CatalogItem.Create(Guid.NewGuid(), "Laptop", "Gaming laptop");
    item.ClearEvents();

    item.Reject("Invalid information");

    Assert.Equal(ItemStatus.Rejected, item.Status);
    var rejectedEvent = Assert.IsType<ItemRejectedEvent>(Assert.Single(item.DomainEvents));
    Assert.Equal(item.Id, rejectedEvent.AggregateId);
    Assert.Equal(item.OwnerId, rejectedEvent.OwnerId);
    Assert.Equal("Invalid information", rejectedEvent.Reason);
  }

  [Fact]
  public void Reject_WhenNotPending_ThrowsDomainException() {
    var item = CatalogItem.Create(Guid.NewGuid(), "Laptop", "Gaming laptop");
    item.Approve();

    var exception = Assert.Throws<DomainException>(() => item.Reject("Invalid information"));

    Assert.Equal("Chỉ có thể từ chối sản phẩm đang chờ duyệt.", exception.Message);
  }

  [Fact]
  public void Revoke_WhenPending_ChangesStatusToRevoked() {
    var item = CatalogItem.Create(Guid.NewGuid(), "Laptop", "Gaming laptop");

    item.Revoke();

    Assert.Equal(ItemStatus.Revoked, item.Status);
  }

  [Fact]
  public void Revoke_WhenNotPending_ThrowsDomainException() {
    var item = CatalogItem.Create(Guid.NewGuid(), "Laptop", "Gaming laptop");
    item.Approve();

    var exception = Assert.Throws<DomainException>(() => item.Revoke());

    Assert.Equal("Chỉ có thể thu hồi sản phẩm đang chờ duyệt.", exception.Message);
  }

  [Fact]
  public void Rejoin_WhenRevoked_LeavesStatusAsRevoked() {
    var item = CatalogItem.Create(Guid.NewGuid(), "Laptop", "Gaming laptop");
    item.Revoke();

    item.Rejoin();

    Assert.Equal(ItemStatus.Revoked, item.Status);
  }

  [Fact]
  public void Rejoin_WhenNotRevoked_ThrowsDomainException() {
    var item = CatalogItem.Create(Guid.NewGuid(), "Laptop", "Gaming laptop");

    var exception = Assert.Throws<DomainException>(() => item.Rejoin());

    Assert.Equal("Chỉ có thể gửi duyệt sản phẩm đã thu hồi.", exception.Message);
  }

  [Fact]
  public void Approve_WhenPending_ChangesStatusAndRaisesApprovedEvent() {
    var item = CatalogItem.Create(Guid.NewGuid(), "Laptop", "Gaming laptop");
    item.ClearEvents();

    item.Approve();

    Assert.Equal(ItemStatus.Approval, item.Status);
    var approvedEvent = Assert.IsType<ItemApprovedEvent>(Assert.Single(item.DomainEvents));
    Assert.Equal(item.Id, approvedEvent.AggregateId);
    Assert.Equal(item.OwnerId, approvedEvent.OwnerId);
  }

  [Fact]
  public void Approve_WhenNotPending_ThrowsDomainException() {
    var item = CatalogItem.Create(Guid.NewGuid(), "Laptop", "Gaming laptop");
    item.Revoke();

    var exception = Assert.Throws<DomainException>(() => item.Approve());

    Assert.Equal("Chỉ có thể duyệt sản phẩm đang chờ duyệt.", exception.Message);
  }

  [Theory]
  [InlineData(true, ItemStatus.Sold)]
  [InlineData(false, ItemStatus.Unsold)]
  public void Sell_ChangesStatusBasedOnSoldFlag(bool isSold, ItemStatus expectedStatus) {
    var item = CatalogItem.Create(Guid.NewGuid(), "Laptop", "Gaming laptop");

    item.Sell(isSold);

    Assert.Equal(expectedStatus, item.Status);
  }
}
