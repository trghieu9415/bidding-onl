using FluentAssertions;
using L1.Core.Domain.Transaction.Entities;
using L1.Core.Domain.Transaction.Enums;
using L2.Application.Exceptions;
using L2.Application.Repositories;
using L2.Application.UseCases.Transactions.Commands.RevertPayment;
using NSubstitute;
using Tests.Common.Builders;
using Xunit;

namespace Tests.Unit.L2.Application.UseCases.Transactions.Commands;

public class RevertPaymentHandlerTests {
  private readonly IRepository<Payment> _paymentRepository = Substitute.For<IRepository<Payment>>();
  private readonly RevertPaymentHandler _sut;

  public RevertPaymentHandlerTests() {
    _sut = new RevertPaymentHandler(_paymentRepository);
  }

  [Fact]
  public async Task Handle_Should_ThrowWorkflowException_When_PaymentNotFound() {
    var request = new RevertPaymentCommand(Guid.NewGuid());

    _paymentRepository.GetByIdAsync(request.Id, CancellationToken.None).Returns((Payment?)null);

    var act = async () => await _sut.Handle(request, CancellationToken.None);

    var exception = await act.Should().ThrowAsync<WorkflowException>();
    exception.Which.StatusCode.Should().Be(404);
    exception.Which.Message.Should().Be("Không tìm thấy thanh toán");
  }

  [Fact]
  public async Task Handle_Should_RefundPayment_And_ReturnTrue() {
    var payment = new PaymentBuilder().Build();
    payment.MarkAsCompleted("txn_123");
    var request = new RevertPaymentCommand(payment.Id);

    _paymentRepository.GetByIdAsync(request.Id, CancellationToken.None).Returns(payment);

    var result = await _sut.Handle(request, CancellationToken.None);

    result.Should().BeTrue();
    payment.Status.Should().Be(PaymentStatus.Refunded);
    await _paymentRepository.Received(1).UpdateAsync(payment, CancellationToken.None);
  }
}
