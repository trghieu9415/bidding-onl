using FluentAssertions;
using FluentValidation.TestHelper;
using L1.Core.Domain.Transaction.Enums;
using L2.Application.UseCases.Transactions.Commands.CreatePayment;
using Xunit;

namespace Tests.Unit.L2.Application.UseCases.Transactions.Validators;

public class CreatePaymentValidatorTests {
  private readonly CreatePaymentValidator _validator = new();

  [Fact]
  public void Should_Have_Error_When_OrderId_Is_Empty() {
    var command = new CreatePaymentCommand(Guid.Empty, Guid.NewGuid(), PaymentMethod.Stripe);

    var result = _validator.TestValidate(command);

    result.ShouldHaveValidationErrorFor(x => x.OrderId)
      .WithErrorMessage("Id đơn hàng không được để trống");
  }

  [Fact]
  public void Should_Have_Error_When_Method_Is_Invalid() {
    var command = new CreatePaymentCommand(Guid.NewGuid(), Guid.NewGuid(), (PaymentMethod)999);

    var result = _validator.TestValidate(command);

    result.ShouldHaveValidationErrorFor(x => x.Method)
      .WithErrorMessage("Phương thức thanh toán không hợp lệ");
  }

  [Fact]
  public void Should_Not_Have_Error_When_Command_Is_Valid() {
    var command = new CreatePaymentCommand(Guid.NewGuid(), Guid.NewGuid(), PaymentMethod.Paypal);

    var result = _validator.TestValidate(command);

    result.IsValid.Should().BeTrue();
  }
}
