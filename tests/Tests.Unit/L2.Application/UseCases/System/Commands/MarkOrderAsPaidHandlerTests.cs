using FluentAssertions;
using L1.Core.Domain.Transaction.Entities;
using L1.Core.Domain.Transaction.Enums;
using L2.Application.Exceptions;
using L2.Application.Repositories;
using L2.Application.UseCases.System.MarkOrderAsPaid;
using NSubstitute;
using Tests.Common.Builders;
using Xunit;

namespace Tests.Unit.L2.Application.UseCases.System.Commands;

public class MarkOrderAsPaidHandlerTests {
  private readonly IRepository<Order> _repository = Substitute.For<IRepository<Order>>();
  private readonly MarkOrderAsPaidHandler _sut;

  public MarkOrderAsPaidHandlerTests() {
    _sut = new MarkOrderAsPaidHandler(_repository);
  }

  [Fact]
  public async Task Handle_Should_ThrowWorkflowException_When_OrderNotFound() {
    var request = new MarkOrderAsPaidCommand(Guid.NewGuid());

    _repository.GetByIdAsync(request.Id, CancellationToken.None).Returns((Order?)null);

    var act = async () => await _sut.Handle(request, CancellationToken.None);

    var exception = await act.Should().ThrowAsync<WorkflowException>();
    exception.Which.StatusCode.Should().Be(404);
    exception.Which.Message.Should().Be("Đơn hàng không tồn tại");
  }

  [Fact]
  public async Task Handle_Should_MarkOrderAsPaid_And_ReturnTrue() {
    var order = new OrderBuilder().Build();
    var request = new MarkOrderAsPaidCommand(order.Id);

    _repository.GetByIdAsync(request.Id, CancellationToken.None).Returns(order);

    var result = await _sut.Handle(request, CancellationToken.None);

    result.Should().BeTrue();
    order.Status.Should().Be(OrderStatus.Confirmed);
    await _repository.Received(1).UpdateAsync(order, CancellationToken.None);
  }
}
