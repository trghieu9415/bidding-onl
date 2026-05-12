using FluentAssertions;
using L1.Core.Domain.Transaction.Entities;
using L1.Core.Domain.Transaction.Enums;
using L2.Application.Ports.Gateway;
using L2.Application.Repositories;
using L2.Application.UseCases.System.RefundPayment;
using NSubstitute;
using Tests.Common.Builders;
using Xunit;

namespace Tests.Unit.L2.Application.UseCases.System.Commands;

public class RefundPaymentHandlerTests {
  private readonly IRepository<Payment> _paymentRepository = Substitute.For<IRepository<Payment>>();
  private readonly IGatewayFactory _gatewayFactory = Substitute.For<IGatewayFactory>();
  private readonly IPaymentGateway _paymentGateway = Substitute.For<IPaymentGateway>();
  private readonly RefundPaymentHandler _sut;

  public RefundPaymentHandlerTests() {
    _sut = new RefundPaymentHandler(_paymentRepository, _gatewayFactory);
  }

  [Fact]
  public async Task Handle_Should_ReturnFalse_When_PaymentNotFound() {
    var request = new RefundPaymentCommand(Guid.NewGuid());

    _paymentRepository.GetByIdAsync(request.Id, CancellationToken.None).Returns((Payment?)null);

    var result = await _sut.Handle(request, CancellationToken.None);

    result.Should().BeFalse();
  }

  [Fact]
  public async Task Handle_Should_RefundPayment_And_ReturnGatewayResult() {
    var payment = new PaymentBuilder().WithMethod(PaymentMethod.Stripe).Build();
    payment.MarkAsCompleted("txn_123");
    var request = new RefundPaymentCommand(payment.Id);

    _paymentRepository.GetByIdAsync(request.Id, CancellationToken.None).Returns(payment);
    _gatewayFactory.CreatePaymentGateway(payment.Method).Returns(_paymentGateway);
    _paymentGateway.RefundPayment(payment, CancellationToken.None).Returns(true);

    var result = await _sut.Handle(request, CancellationToken.None);

    result.Should().BeTrue();
    payment.Status.Should().Be(PaymentStatus.Refunded);
    await _paymentRepository.Received(1).UpdateAsync(payment, CancellationToken.None);
  }
}
