using System.Text.Json;
using FluentAssertions;
using L1.Core.Domain.Transaction.Entities;
using L1.Core.Domain.Transaction.Enums;
using L2.Application.Ports.Gateway;
using L2.Application.Repositories;
using L2.Application.UseCases.Transactions.Commands.ProcessPayment;
using NSubstitute;
using Tests.Common.Builders;
using Tests.Unit.L2.Application.UseCases;
using Xunit;

namespace Tests.Unit.L2.Application.UseCases.Transactions.Commands;

public class ProcessPaymentHandlerTests {
  private readonly IRepository<Payment> _paymentRepository = Substitute.For<IRepository<Payment>>();
  private readonly IGatewayFactory _gatewayFactory = Substitute.For<IGatewayFactory>();
  private readonly IPaymentGateway _paymentGateway = Substitute.For<IPaymentGateway>();
  private readonly ProcessPaymentHandler _sut;

  public ProcessPaymentHandlerTests() {
    _sut = new ProcessPaymentHandler(_paymentRepository, _gatewayFactory);
  }

  [Fact]
  public async Task Handle_Should_ReturnFalse_When_WebhookVerificationFails() {
    var request = new ProcessPaymentCommand(new ProcessPaymentRequest(PaymentMethod.Stripe, CreatePayload()));

    _gatewayFactory.CreatePaymentGateway(request.Data.Method).Returns(_paymentGateway);
    _paymentGateway.ToWebhookPayload(request.Data.Payload).Returns(new TestWebhookPayload());
    _paymentGateway.VerifyWebhookPayment(Arg.Any<TestWebhookPayload>(), CancellationToken.None)
      .Returns((false, Guid.NewGuid(), string.Empty));

    var result = await _sut.Handle(request, CancellationToken.None);

    result.Should().BeFalse();
  }

  [Fact]
  public async Task Handle_Should_ReturnTrue_When_PaymentRecordNotFound() {
    var paymentId = Guid.NewGuid();
    var request = new ProcessPaymentCommand(new ProcessPaymentRequest(PaymentMethod.Stripe, CreatePayload()));

    _gatewayFactory.CreatePaymentGateway(request.Data.Method).Returns(_paymentGateway);
    _paymentGateway.ToWebhookPayload(request.Data.Payload).Returns(new TestWebhookPayload());
    _paymentGateway.VerifyWebhookPayment(Arg.Any<TestWebhookPayload>(), CancellationToken.None)
      .Returns((true, paymentId, "txn_123"));
    _paymentRepository.GetByIdAsync(paymentId, CancellationToken.None).Returns((Payment?)null);

    var result = await _sut.Handle(request, CancellationToken.None);

    result.Should().BeTrue();
  }

  [Fact]
  public async Task Handle_Should_ReturnTrue_When_PaymentAlreadySucceeded() {
    var payment = new PaymentBuilder().Build();
    payment.MarkAsCompleted("txn_old");
    var request = new ProcessPaymentCommand(new ProcessPaymentRequest(PaymentMethod.Stripe, CreatePayload()));

    _gatewayFactory.CreatePaymentGateway(request.Data.Method).Returns(_paymentGateway);
    _paymentGateway.ToWebhookPayload(request.Data.Payload).Returns(new TestWebhookPayload());
    _paymentGateway.VerifyWebhookPayment(Arg.Any<TestWebhookPayload>(), CancellationToken.None)
      .Returns((true, payment.Id, "txn_new"));
    _paymentRepository.GetByIdAsync(payment.Id, CancellationToken.None).Returns(payment);

    var result = await _sut.Handle(request, CancellationToken.None);

    result.Should().BeTrue();
    payment.TransactionId.Should().Be("txn_old");
  }

  [Fact]
  public async Task Handle_Should_MarkPaymentAsCompleted_When_VerificationSucceeds() {
    var payment = new PaymentBuilder().Build();
    var request = new ProcessPaymentCommand(new ProcessPaymentRequest(PaymentMethod.Stripe, CreatePayload()));

    _gatewayFactory.CreatePaymentGateway(request.Data.Method).Returns(_paymentGateway);
    _paymentGateway.ToWebhookPayload(request.Data.Payload).Returns(new TestWebhookPayload());
    _paymentGateway.VerifyWebhookPayment(Arg.Any<TestWebhookPayload>(), CancellationToken.None)
      .Returns((true, payment.Id, "txn_123"));
    _paymentRepository.GetByIdAsync(payment.Id, CancellationToken.None).Returns(payment);

    var result = await _sut.Handle(request, CancellationToken.None);

    result.Should().BeTrue();
    payment.Status.Should().Be(PaymentStatus.Succeeded);
    payment.TransactionId.Should().Be("txn_123");
  }

  private static JsonElement CreatePayload() {
    using var document = JsonDocument.Parse("{}");
    return document.RootElement.Clone();
  }
}
