using FluentAssertions;
using L1.Core.Domain.Catalog.Entities;
using L1.Core.Domain.Catalog.Enums;
using L2.Application.Exceptions;
using L2.Application.Repositories;
using L2.Application.UseCases.Items.Commands.RejoinItem;
using NSubstitute;
using Tests.Common.Builders;
using Xunit;

namespace Tests.Unit.L2.Application.UseCases.Items.Commands;

public class RejoinItemHandlerTests {
  private readonly IRepository<CatalogItem> _repository = Substitute.For<IRepository<CatalogItem>>();
  private readonly RejoinItemHandler _sut;

  public RejoinItemHandlerTests() {
    _sut = new RejoinItemHandler(_repository);
  }

  [Fact]
  public async Task Handle_Should_ThrowWorkflowException_When_ItemMissingOrNotOwned() {
    var request = new RejoinItemCommand(Guid.NewGuid(), Guid.NewGuid());

    _repository.GetByIdAsync(request.Id, CancellationToken.None).Returns((CatalogItem?)null);

    var act = async () => await _sut.Handle(request, CancellationToken.None);

    var exception = await act.Should().ThrowAsync<WorkflowException>();
    exception.Which.Message.Should().Be("Không tìm thấy sản phẩm");
  }

  [Fact]
  public async Task Handle_Should_RejoinItem_And_ReturnTrue() {
    var item = new CatalogItemBuilder().WithOwnerId(Guid.NewGuid()).Build();
    item.Revoke();
    var request = new RejoinItemCommand(item.Id, item.OwnerId);

    _repository.GetByIdAsync(request.Id, CancellationToken.None).Returns(item);

    var result = await _sut.Handle(request, CancellationToken.None);

    result.Should().BeTrue();
    item.Status.Should().Be(ItemStatus.Revoked);
    await _repository.Received(1).UpdateAsync(item, CancellationToken.None);
  }
}
