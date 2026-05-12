using FluentAssertions;
using L1.Core.Domain.Catalog.Entities;
using L1.Core.Domain.Catalog.Enums;
using L2.Application.Exceptions;
using L2.Application.Repositories;
using L2.Application.UseCases.Items.Commands.UpdateRegisteredItem;
using NSubstitute;
using Tests.Common.Builders;
using Xunit;

namespace Tests.Unit.L2.Application.UseCases.Items.Commands;

public class UpdateRegisteredItemHandlerTests {
  private readonly IRepository<CatalogItem> _repository = Substitute.For<IRepository<CatalogItem>>();
  private readonly UpdateRegisteredItemHandler _sut;

  public UpdateRegisteredItemHandlerTests() {
    _sut = new UpdateRegisteredItemHandler(_repository);
  }

  [Fact]
  public async Task Handle_Should_ThrowWorkflowException_When_ItemNotFound() {
    var request = new UpdateRegisteredItemCommand(Guid.NewGuid(), Guid.NewGuid(), new UpdateRegisteredItemRequest(null, null, null, null, null, null, null));

    _repository.GetByIdAsync(request.Id, CancellationToken.None).Returns((CatalogItem?)null);

    var act = async () => await _sut.Handle(request, CancellationToken.None);

    var exception = await act.Should().ThrowAsync<WorkflowException>();
    exception.Which.StatusCode.Should().Be(404);
    exception.Which.Message.Should().Be("Sản phẩm không tồn tại");
  }

  [Fact]
  public async Task Handle_Should_ThrowWorkflowException_When_UserDoesNotOwnItem() {
    var item = new CatalogItemBuilder().WithOwnerId(Guid.NewGuid()).Build();
    var request = new UpdateRegisteredItemCommand(item.Id, Guid.NewGuid(), new UpdateRegisteredItemRequest(null, null, null, null, null, null, null));

    _repository.GetByIdAsync(request.Id, CancellationToken.None).Returns(item);

    var act = async () => await _sut.Handle(request, CancellationToken.None);

    var exception = await act.Should().ThrowAsync<WorkflowException>();
    exception.Which.StatusCode.Should().Be(403);
    exception.Which.Message.Should().Be("Bạn không có quyền chỉnh sửa sản phẩm này");
  }

  [Fact]
  public async Task Handle_Should_ThrowWorkflowException_When_ItemStatusIsNotEditable() {
    var item = new CatalogItemBuilder().WithOwnerId(Guid.NewGuid()).Build();
    item.Approve();
    var request = new UpdateRegisteredItemCommand(item.Id, item.OwnerId, new UpdateRegisteredItemRequest(null, null, null, null, null, null, null));

    _repository.GetByIdAsync(request.Id, CancellationToken.None).Returns(item);

    var act = async () => await _sut.Handle(request, CancellationToken.None);

    var exception = await act.Should().ThrowAsync<WorkflowException>();
    exception.Which.Message.Should().Be("Không thể sửa sản phẩm đã được duyệt hoặc đã bán");
  }

  [Fact]
  public async Task Handle_Should_UpdateRegisteredItem_And_ReturnTrue() {
    var item = new CatalogItemBuilder().WithOwnerId(Guid.NewGuid()).Build();
    var categoryId = Guid.NewGuid();
    var request = new UpdateRegisteredItemCommand(
      item.Id,
      item.OwnerId,
      new UpdateRegisteredItemRequest(
        "New Name",
        "New Description",
        2000,
        ItemCondition.OpenBox,
        [categoryId],
        "https://example.com/new-main.jpg",
        ["https://example.com/new-sub.jpg"]
      )
    );

    _repository.GetByIdAsync(request.Id, CancellationToken.None).Returns(item);

    var result = await _sut.Handle(request, CancellationToken.None);

    result.Should().BeTrue();
    item.Name.Should().Be("New Name");
    item.Description.Should().Be("New Description");
    item.StartingPrice.Should().Be(2000);
    item.Condition.Should().Be(ItemCondition.OpenBox);
    item.CategoryIds.Should().Contain(categoryId);
    item.Images.MainImageUrl.Should().Be("https://example.com/new-main.jpg");
    await _repository.Received(1).UpdateAsync(item, CancellationToken.None);
  }
}
