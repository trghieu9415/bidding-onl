using FluentAssertions;
using L1.Core.Domain.Catalog.Entities;
using L1.Core.Domain.Catalog.Enums;
using L2.Application.Exceptions;
using L2.Application.Repositories;
using L2.Application.UseCases.Items.Commands.ApproveItem;
using NSubstitute;
using Tests.Common.Builders;
using Xunit;

namespace Tests.Unit.L2.Application.UseCases.Items.Commands;

public class ApproveItemHandlerTests {
  private readonly IRepository<CatalogItem> _repository = Substitute.For<IRepository<CatalogItem>>();
  private readonly ApproveItemHandler _sut;

  public ApproveItemHandlerTests() {
    _sut = new ApproveItemHandler(_repository);
  }

  [Fact]
  public async Task Handle_Should_ThrowWorkflowException_When_ItemNotFound() {
    var request = new ApproveItemCommand(Guid.NewGuid());

    _repository.GetByIdAsync(request.Id, CancellationToken.None).Returns((CatalogItem?)null);

    var act = async () => await _sut.Handle(request, CancellationToken.None);

    var exception = await act.Should().ThrowAsync<WorkflowException>();
    exception.Which.StatusCode.Should().Be(404);
    exception.Which.Message.Should().Be("Sản phẩm không tồn tại");
  }

  [Fact]
  public async Task Handle_Should_ApproveItem_And_ReturnTrue() {
    var item = new CatalogItemBuilder().Build();
    var request = new ApproveItemCommand(item.Id);

    _repository.GetByIdAsync(request.Id, CancellationToken.None).Returns(item);

    var result = await _sut.Handle(request, CancellationToken.None);

    result.Should().BeTrue();
    item.Status.Should().Be(ItemStatus.Approval);
    await _repository.Received(1).UpdateAsync(item, CancellationToken.None);
  }
}
