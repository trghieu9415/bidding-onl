using FluentAssertions;
using L1.Core.Domain.Transaction.Entities;
using L1.Core.Domain.Transaction.Enums;
using L2.Application.Exceptions;
using L2.Application.Repositories;
using L2.Application.UseCases.System.RefundOrder;
using NSubstitute;
using Tests.Common.Builders;
using Xunit;

namespace Tests.Unit.L2.Application.UseCases.System.Commands;

public class RefundOrderHandlerTests {
  private readonly IRepository<Order> _repository = Substitute.For<IRepository<Order>>();
  private readonly RefundOrderHandler _sut;

  public RefundOrderHandlerTests() {
    _sut = new RefundOrderHandler(_repository);
  }

  [Fact]
  public async Task Handle_Should_ThrowWorkflowException_When_OrderNotFound() {
    var request = new RefundOrderCommand(Guid.NewGuid());

    _repository.GetByIdAsync(request.Id, CancellationToken.None).Returns((Order?)null);

    var act = async () => await _sut.Handle(request, CancellationToken.None);

    var exception = await act.Should().ThrowAsync<WorkflowException>();
    exception.Which.StatusCode.Should().Be(404);
    exception.Which.Message.Should().Be("Đơn hàng không tồn tại");
  }

  [Fact]
  public async Task Handle_Should_RefundOrder_And_ReturnTrue() {
    var order = new OrderBuilder().Build();
    order.MarkAsPaid(order.BidderEmail);
    var request = new RefundOrderCommand(order.Id);

    _repository.GetByIdAsync(request.Id, CancellationToken.None).Returns(order);

    var result = await _sut.Handle(request, CancellationToken.None);

    result.Should().BeTrue();
    order.Status.Should().Be(OrderStatus.Refunded);
    await _repository.Received(1).UpdateAsync(order, CancellationToken.None);
  }
}
