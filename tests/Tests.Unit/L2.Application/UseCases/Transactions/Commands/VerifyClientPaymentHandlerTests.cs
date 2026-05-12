using System.Text.Json;
using FluentAssertions;
using L1.Core.Domain.Transaction.Entities;
using L1.Core.Domain.Transaction.Enums;
using L2.Application.Ports.Gateway;
using L2.Application.Repositories;
using L2.Application.UseCases.Transactions.Commands.VerifyPayment;
using NSubstitute;
using Tests.Common.Builders;
using Tests.Unit.L2.Application.UseCases;
using Xunit;

namespace Tests.Unit.L2.Application.UseCases.Transactions.Commands;

public class VerifyClientPaymentHandlerTests {
  private readonly IRepository<Payment> _paymentRepository = Substitute.For<IRepository<Payment>>();
  private readonly IGatewayFactory _gatewayFactory = Substitute.For<IGatewayFactory>();
  private readonly IPaymentGateway _paymentGateway = Substitute.For<IPaymentGateway>();
  private readonly VerifyClientPaymentHandler _sut;

  public VerifyClientPaymentHandlerTests() {
    _sut = new VerifyClientPaymentHandler(_paymentRepository, _gatewayFactory);
  }

  [Fact]
  public async Task Handle_Should_ReturnFalse_When_PaymentNotFound() {
    var request = new VerifyClientPaymentCommand(Guid.NewGuid(), new VerifyClientPaymentRequest(Guid.NewGuid(), CreatePayload()));

    _paymentRepository.GetByIdAsync(request.Data.Id, CancellationToken.None).Returns((Payment?)null);

    var result = await _sut.Handle(request, CancellationToken.None);

    result.Should().BeFalse();
  }

  [Fact]
  public async Task Handle_Should_ReturnTrue_When_PaymentAlreadySucceeded() {
    var payment = new PaymentBuilder().Build();
    payment.MarkAsCompleted("txn_old");
    var request = new VerifyClientPaymentCommand(Guid.NewGuid(), new VerifyClientPaymentRequest(payment.Id, CreatePayload()));

    _paymentRepository.GetByIdAsync(request.Data.Id, CancellationToken.None).Returns(payment);

    var result = await _sut.Handle(request, CancellationToken.None);

    result.Should().BeTrue();
    payment.TransactionId.Should().Be("txn_old");
  }

  [Fact]
  public async Task Handle_Should_MarkPaymentAsCompleted_When_ClientVerificationSucceeds() {
    var payment = new PaymentBuilder().Build();
    var request = new VerifyClientPaymentCommand(Guid.NewGuid(), new VerifyClientPaymentRequest(payment.Id, CreatePayload()));

    _paymentRepository.GetByIdAsync(request.Data.Id, CancellationToken.None).Returns(payment);
    _gatewayFactory.CreatePaymentGateway(payment.Method).Returns(_paymentGateway);
    _paymentGateway.ToClientPayload(request.Data.Payload).Returns(new TestClientPayload());
    _paymentGateway.VerifyClientPayment(Arg.Any<TestClientPayload>(), CancellationToken.None)
      .Returns((true, "txn_123"));

    var result = await _sut.Handle(request, CancellationToken.None);

    result.Should().BeTrue();
    payment.Status.Should().Be(PaymentStatus.Succeeded);
  }

  [Fact]
  public async Task Handle_Should_MarkPaymentAsFailed_When_ClientVerificationFails() {
    var payment = new PaymentBuilder().Build();
    var request = new VerifyClientPaymentCommand(Guid.NewGuid(), new VerifyClientPaymentRequest(payment.Id, CreatePayload()));

    _paymentRepository.GetByIdAsync(request.Data.Id, CancellationToken.None).Returns(payment);
    _gatewayFactory.CreatePaymentGateway(payment.Method).Returns(_paymentGateway);
    _paymentGateway.ToClientPayload(request.Data.Payload).Returns(new TestClientPayload());
    _paymentGateway.VerifyClientPayment(Arg.Any<TestClientPayload>(), CancellationToken.None)
      .Returns((false, string.Empty));

    var result = await _sut.Handle(request, CancellationToken.None);

    result.Should().BeTrue();
    payment.Status.Should().Be(PaymentStatus.Failed);
  }

  private static JsonElement CreatePayload() {
    using var document = JsonDocument.Parse("{}");
    return document.RootElement.Clone();
  }
}
