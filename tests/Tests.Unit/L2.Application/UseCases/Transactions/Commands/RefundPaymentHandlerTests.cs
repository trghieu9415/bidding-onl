using System.Linq.Expressions;
using FluentAssertions;
using L1.Core.Domain.Transaction.Entities;
using L1.Core.Domain.Transaction.Enums;
using L2.Application.Exceptions;
using L2.Application.Repositories;
using L2.Application.Repositories.Read;
using L2.Application.UseCases.Transactions.Commands.RefundPayment;
using NSubstitute;
using Tests.Common.Builders;
using Xunit;

namespace Tests.Unit.L2.Application.UseCases.Transactions.Commands;

public class RefundPaymentHandlerTests {
  private readonly IOrderReadRepository _orderReadRepository = Substitute.For<IOrderReadRepository>();
  private readonly IRepository<Payment> _paymentRepository = Substitute.For<IRepository<Payment>>();
  private readonly RefundPaymentHandler _sut;

  public RefundPaymentHandlerTests() {
    _sut = new RefundPaymentHandler(_paymentRepository, _orderReadRepository);
  }

  [Fact]
  public async Task Handle_Should_ThrowWorkflowException_When_PaymentNotFound() {
    var request = new RefundPaymentCommand(Guid.NewGuid(), Guid.NewGuid());

    _paymentRepository.GetByIdAsync(request.Id, CancellationToken.None).Returns((Payment?)null);

    var act = async () => await _sut.Handle(request, CancellationToken.None);

    var exception = await act.Should().ThrowAsync<WorkflowException>();
    exception.Which.StatusCode.Should().Be(404);
    exception.Which.Message.Should().Be("Không tìm thấy thanh toán");
  }

  [Fact]
  public async Task Handle_Should_ThrowWorkflowException_When_UserDoesNotOwnOrder() {
    var payment = new PaymentBuilder().Build();
    payment.MarkAsCompleted("txn_123");
    var request = new RefundPaymentCommand(payment.Id, Guid.NewGuid());
    var order = UseCaseTestData.CreateOrderDto(Guid.NewGuid(), Guid.NewGuid());

    _paymentRepository.GetByIdAsync(request.Id, CancellationToken.None).Returns(payment);
    _orderReadRepository.GetFirstAsync(Arg.Any<Expression<Func<Order, bool>>>(), CancellationToken.None)
      .Returns(order);

    var act = async () => await _sut.Handle(request, CancellationToken.None);

    var exception = await act.Should().ThrowAsync<WorkflowException>();
    exception.Which.StatusCode.Should().Be(403);
    exception.Which.Message.Should().Be("Bạn không có quyền hoàn trả đơn hàng này");
  }

  [Fact]
  public async Task Handle_Should_RefundPayment_And_ReturnTrue() {
    var userId = Guid.NewGuid();
    var payment = new PaymentBuilder().Build();
    payment.MarkAsCompleted("txn_123");
    var request = new RefundPaymentCommand(payment.Id, userId);
    var order = UseCaseTestData.CreateOrderDto(Guid.NewGuid(), userId);

    _paymentRepository.GetByIdAsync(request.Id, CancellationToken.None).Returns(payment);
    _orderReadRepository.GetFirstAsync(Arg.Any<Expression<Func<Order, bool>>>(), CancellationToken.None)
      .Returns(order);

    var result = await _sut.Handle(request, CancellationToken.None);

    result.Should().BeTrue();
    payment.Status.Should().Be(PaymentStatus.Refunded);
    await _paymentRepository.Received(1).UpdateAsync(payment, CancellationToken.None);
  }
}
