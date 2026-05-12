using FluentAssertions;
using L1.Core.Domain.Catalog.Entities;
using L1.Core.Domain.Catalog.Enums;
using L2.Application.Exceptions;
using L2.Application.Repositories;
using L2.Application.UseCases.Items.Commands.RejectItem;
using NSubstitute;
using Tests.Common.Builders;
using Xunit;

namespace Tests.Unit.L2.Application.UseCases.Items.Commands;

public class RejectItemHandlerTests {
  private readonly IRepository<CatalogItem> _repository = Substitute.For<IRepository<CatalogItem>>();
  private readonly RejectItemHandler _sut;

  public RejectItemHandlerTests() {
    _sut = new RejectItemHandler(_repository);
  }

  [Fact]
  public async Task Handle_Should_ThrowWorkflowException_When_ItemNotFound() {
    var request = new RejectItemCommand(Guid.NewGuid(), new RejectItemRequest("Invalid"));

    _repository.GetByIdAsync(request.Id, CancellationToken.None).Returns((CatalogItem?)null);

    var act = async () => await _sut.Handle(request, CancellationToken.None);

    var exception = await act.Should().ThrowAsync<WorkflowException>();
    exception.Which.StatusCode.Should().Be(404);
    exception.Which.Message.Should().Be("Sản phẩm không tồn tại");
  }

  [Fact]
  public async Task Handle_Should_RejectItem_And_ReturnTrue() {
    var item = new CatalogItemBuilder().Build();
    var request = new RejectItemCommand(item.Id, new RejectItemRequest("Invalid"));

    _repository.GetByIdAsync(request.Id, CancellationToken.None).Returns(item);

    var result = await _sut.Handle(request, CancellationToken.None);

    result.Should().BeTrue();
    item.Status.Should().Be(ItemStatus.Rejected);
    await _repository.Received(1).UpdateAsync(item, CancellationToken.None);
  }
}
