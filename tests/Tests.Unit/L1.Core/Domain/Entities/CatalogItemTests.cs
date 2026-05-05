using FluentAssertions;
using L1.Core.Domain.Catalog.Enums;
using L1.Core.Domain.Catalog.Events;
using L1.Core.Exceptions;
using Tests.Common.Builders;
using Xunit;

namespace Tests.Unit.L1.Core.Domain.Entities;

public class CatalogItemTests {
  [Fact]
  public void Create_ValidParameters_InitializesPendingItemAndRaisesRegisteredEvent() {
    // Arrange
    var ownerId = Guid.NewGuid();
    var builder = new CatalogItemBuilder()
      .WithOwnerId(ownerId)
      .WithName("Laptop")
      .WithDescription("Gaming laptop");

    // Act
    var item = builder.Build();

    // Assert
    item.OwnerId.Should().Be(ownerId);
    item.Name.Should().Be("Laptop");
    item.Description.Should().Be("Gaming laptop");
    item.Status.Should().Be(ItemStatus.Pending);
    item.Condition.Should().BeNull();
    item.StartingPrice.Should().Be(0m);
    item.CategoryIds.Should().BeEmpty();
    item.Images.MainImageUrl.Should().BeNull();
    item.Images.SubImageUrls.Should().BeEmpty();

    var registeredEvent = item.DomainEvents.Should().ContainSingle().Subject.As<ItemRegisteredEvent>();
    registeredEvent.AggregateId.Should().Be(item.Id);
    registeredEvent.OwnerId.Should().Be(ownerId);
    registeredEvent.Name.Should().Be("Laptop");
  }

  [Fact]
  public void Update_WithProvidedValues_ChangesFieldsAndReturnsSameItem() {
    // Arrange
    var item = new CatalogItemBuilder().WithName("Laptop").WithDescription("Gaming laptop").Build();

    // Act
    var returnedItem = item.Update("Ultrabook", "Portable laptop");

    // Assert
    returnedItem.Should().BeSameAs(item);
    item.Name.Should().Be("Ultrabook");
    item.Description.Should().Be("Portable laptop");
  }

  [Fact]
  public void Update_WithNullValues_KeepsCurrentFields() {
    // Arrange
    var item = new CatalogItemBuilder().WithName("Laptop").WithDescription("Gaming laptop").Build();

    // Act
    item.Update(null, null);

    // Assert
    item.Name.Should().Be("Laptop");
    item.Description.Should().Be("Gaming laptop");
  }

  [Fact]
  public void SetCondition_SetsConditionAndReturnsSameItem() {
    // Arrange
    var item = new CatalogItemBuilder().Build();

    // Act
    var returnedItem = item.SetCondition(ItemCondition.OpenBox);

    // Assert
    returnedItem.Should().BeSameAs(item);
    item.Condition.Should().Be(ItemCondition.OpenBox);
  }

  [Fact]
  public void SetStartingPrice_SetsPriceAndReturnsSameItem() {
    // Arrange
    var item = new CatalogItemBuilder().Build();

    // Act
    var returnedItem = item.SetStartingPrice(250m);

    // Assert
    returnedItem.Should().BeSameAs(item);
    item.StartingPrice.Should().Be(250m);
  }

  [Fact]
  public void SyncCategories_ReplacesCategoryIdsAndReturnsSameItem() {
    // Arrange
    var item = new CatalogItemBuilder().Build();
    item.SyncCategories([Guid.NewGuid()]);
    var categoryIds = new[] { Guid.NewGuid(), Guid.NewGuid() };

    // Act
    var returnedItem = item.SyncCategories(categoryIds);

    // Assert
    returnedItem.Should().BeSameAs(item);
    item.CategoryIds.Should().BeEquivalentTo(categoryIds);
  }

  [Fact]
  public void SetImageUrls_WithExplicitMainImage_UpdatesGallery() {
    // Arrange
    var item = new CatalogItemBuilder().Build();
    var imageUrls = new[] { "sub-1.png", "sub-2.png" };

    // Act
    var returnedItem = item.SetImageUrls("main.png", imageUrls);

    // Assert
    returnedItem.Should().BeSameAs(item);
    item.Images.MainImageUrl.Should().Be("main.png");
    item.Images.SubImageUrls.Should().BeEquivalentTo(imageUrls);
  }

  [Fact]
  public void SetImageUrls_WhenMainImageIsNull_KeepsCurrentMainImage() {
    // Arrange
    var item = new CatalogItemBuilder().WithImages("main.png", ["sub-1.png"]).Build();

    // Act
    item.SetImageUrls(null, ["sub-2.png"]);

    // Assert
    item.Images.MainImageUrl.Should().Be("main.png");
    item.Images.SubImageUrls.Should().BeEquivalentTo("sub-2.png");
  }

  [Fact]
  public void Reject_WhenPending_ChangesStatusAndRaisesRejectedEvent() {
    // Arrange
    var item = new CatalogItemBuilder().Build();
    item.ClearEvents();

    // Act
    item.Reject("Invalid information");

    // Assert
    item.Status.Should().Be(ItemStatus.Rejected);
    var rejectedEvent = item.DomainEvents.Should().ContainSingle().Subject.As<ItemRejectedEvent>();
    rejectedEvent.AggregateId.Should().Be(item.Id);
    rejectedEvent.OwnerId.Should().Be(item.OwnerId);
    rejectedEvent.Reason.Should().Be("Invalid information");
  }

  [Fact]
  public void Reject_WhenNotPending_ThrowsDomainException() {
    // Arrange
    var item = new CatalogItemBuilder().Build();
    item.Approve();

    // Act
    var act = () => item.Reject("Invalid information");

    // Assert
    act.Should().Throw<DomainException>()
      .WithMessage("Chỉ có thể từ chối sản phẩm đang chờ duyệt.");
  }

  [Fact]
  public void Revoke_WhenPending_ChangesStatusToRevoked() {
    // Arrange
    var item = new CatalogItemBuilder().Build();

    // Act
    item.Revoke();

    // Assert
    item.Status.Should().Be(ItemStatus.Revoked);
  }

  [Fact]
  public void Revoke_WhenNotPending_ThrowsDomainException() {
    // Arrange
    var item = new CatalogItemBuilder().Build();
    item.Approve();

    // Act
    var act = () => item.Revoke();

    // Assert
    act.Should().Throw<DomainException>()
      .WithMessage("Chỉ có thể thu hồi sản phẩm đang chờ duyệt.");
  }

  [Fact]
  public void Rejoin_WhenRevoked_LeavesStatusAsRevoked() {
    // Arrange
    var item = new CatalogItemBuilder().Build();
    item.Revoke();

    // Act
    item.Rejoin();

    // Assert
    item.Status.Should().Be(ItemStatus.Revoked);
  }

  [Fact]
  public void Rejoin_WhenNotRevoked_ThrowsDomainException() {
    // Arrange
    var item = new CatalogItemBuilder().Build();

    // Act
    var act = () => item.Rejoin();

    // Assert
    act.Should().Throw<DomainException>()
      .WithMessage("Chỉ có thể gửi duyệt sản phẩm đã thu hồi.");
  }

  [Fact]
  public void Approve_WhenPending_ChangesStatusAndRaisesApprovedEvent() {
    // Arrange
    var item = new CatalogItemBuilder().Build();
    item.ClearEvents();

    // Act
    item.Approve();

    // Assert
    item.Status.Should().Be(ItemStatus.Approval);
    var approvedEvent = item.DomainEvents.Should().ContainSingle().Subject.As<ItemApprovedEvent>();
    approvedEvent.AggregateId.Should().Be(item.Id);
    approvedEvent.OwnerId.Should().Be(item.OwnerId);
  }

  [Fact]
  public void Approve_WhenNotPending_ThrowsDomainException() {
    // Arrange
    var item = new CatalogItemBuilder().Build();
    item.Revoke();

    // Act
    var act = () => item.Approve();

    // Assert
    act.Should().Throw<DomainException>()
      .WithMessage("Chỉ có thể duyệt sản phẩm đang chờ duyệt.");
  }

  [Theory]
  [InlineData(true, ItemStatus.Sold)]
  [InlineData(false, ItemStatus.Unsold)]
  public void Sell_ChangesStatusBasedOnSoldFlag(bool isSold, ItemStatus expectedStatus) {
    // Arrange
    var item = new CatalogItemBuilder().Build();

    // Act
    item.Sell(isSold);

    // Assert
    item.Status.Should().Be(expectedStatus);
  }
}
