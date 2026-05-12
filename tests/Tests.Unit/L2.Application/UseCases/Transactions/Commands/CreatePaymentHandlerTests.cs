using System.Linq.Expressions;
using FluentAssertions;
using L1.Core.Domain.Transaction.Entities;
using L1.Core.Domain.Transaction.Enums;
using L2.Application.Exceptions;
using L2.Application.Ports.Gateway;
using L2.Application.Repositories;
using L2.Application.UseCases.Transactions.Commands.CreatePayment;
using NSubstitute;
using Tests.Common.Builders;
using Xunit;

namespace Tests.Unit.L2.Application.UseCases.Transactions.Commands;

public class CreatePaymentHandlerTests {
  private readonly IRepository<Order> _orderRepository = Substitute.For<IRepository<Order>>();
  private readonly IRepository<Payment> _paymentRepository = Substitute.For<IRepository<Payment>>();
  private readonly IGatewayFactory _gatewayFactory = Substitute.For<IGatewayFactory>();
  private readonly IPaymentGateway _paymentGateway = Substitute.For<IPaymentGateway>();
  private readonly CreatePaymentHandler _sut;

  public CreatePaymentHandlerTests() {
    _sut = new CreatePaymentHandler(_orderRepository, _paymentRepository, _gatewayFactory);
  }

  [Fact]
  public async Task Handle_Should_ThrowWorkflowException_When_OrderNotFound() {
    var request = new CreatePaymentCommand(Guid.NewGuid(), Guid.NewGuid(), PaymentMethod.Stripe);

    _orderRepository.GetByIdAsync(request.OrderId, CancellationToken.None).Returns((Order?)null);

    var act = async () => await _sut.Handle(request, CancellationToken.None);

    var exception = await act.Should().ThrowAsync<WorkflowException>();
    exception.Which.StatusCode.Should().Be(404);
    exception.Which.Message.Should().Be("Đơn hàng không tồn tại");
  }

  [Fact]
  public async Task Handle_Should_ThrowWorkflowException_When_UserDoesNotOwnOrder() {
    var order = new OrderBuilder().WithBidderId(Guid.NewGuid()).Build();
    var request = new CreatePaymentCommand(order.Id, Guid.NewGuid(), PaymentMethod.Stripe);

    _orderRepository.GetByIdAsync(request.OrderId, CancellationToken.None).Returns(order);

    var act = async () => await _sut.Handle(request, CancellationToken.None);

    var exception = await act.Should().ThrowAsync<WorkflowException>();
    exception.Which.StatusCode.Should().Be(403);
    exception.Which.Message.Should().Be("Bạn không có quyền thanh toán đơn hàng này");
  }

  [Fact]
  public async Task Handle_Should_ReturnExistingPendingPaymentUrl_When_PaymentAlreadyPending() {
    var userId = Guid.NewGuid();
    var order = new OrderBuilder().WithBidderId(userId).Build();
    var payment = new PaymentBuilder().WithOrderId(order.Id).Build();
    payment.SetPaymentUrl("https://existing-payment");
    var request = new CreatePaymentCommand(order.Id, userId, PaymentMethod.Stripe);

    _orderRepository.GetByIdAsync(request.OrderId, CancellationToken.None).Returns(order);
    _paymentRepository.GetFirstAsync(Arg.Any<Expression<Func<Payment, bool>>>(), CancellationToken.None)
      .Returns(payment);

    var result = await _sut.Handle(request, CancellationToken.None);

    result.PaymentUrl.Should().Be("https://existing-payment");
    await _paymentRepository.DidNotReceive().CreateAsync(Arg.Any<Payment>(), CancellationToken.None);
  }

  [Fact]
  public async Task Handle_Should_CreateNewPayment_When_NoPendingPaymentExists() {
    var userId = Guid.NewGuid();
    var order = new OrderBuilder().WithBidderId(userId).Build();
    var request = new CreatePaymentCommand(order.Id, userId, PaymentMethod.Paypal);

    _orderRepository.GetByIdAsync(request.OrderId, CancellationToken.None).Returns(order);
    _paymentRepository.GetFirstAsync(Arg.Any<Expression<Func<Payment, bool>>>(), CancellationToken.None)
      .Returns((Payment?)null);
    _gatewayFactory.CreatePaymentGateway(request.Method).Returns(_paymentGateway);
    _paymentGateway.CreatePaymentUrl(Arg.Any<Payment>(), order, CancellationToken.None).Returns("https://new-payment");

    var result = await _sut.Handle(request, CancellationToken.None);

    result.PaymentUrl.Should().Be("https://new-payment");
    await _paymentRepository.Received(1).CreateAsync(
      Arg.Is<Payment>(x =>
        x.OrderId == order.Id &&
        x.Method == request.Method &&
        x.PaymentUrl == "https://new-payment"
      ),
      CancellationToken.None
    );
  }
}
