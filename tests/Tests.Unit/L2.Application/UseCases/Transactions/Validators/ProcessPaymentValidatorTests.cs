using System.Text.Json;
using FluentAssertions;
using FluentValidation.TestHelper;
using L1.Core.Domain.Transaction.Enums;
using L2.Application.UseCases.Transactions.Commands.ProcessPayment;
using Xunit;

namespace Tests.Unit.L2.Application.UseCases.Transactions.Validators;

public class ProcessPaymentValidatorTests {
  private readonly ProcessPaymentValidator _validator = new();

  [Fact]
  public void Should_Have_Error_When_Method_Is_Invalid() {
    var request = new ProcessPaymentRequest((PaymentMethod)999, CreateValidPayload());

    var result = _validator.TestValidate(request);

    result.ShouldHaveValidationErrorFor(x => x.Method)
      .WithErrorMessage("Phương thức thanh toán không hợp lệ.");
  }

  [Fact]
  public void Should_Have_Error_When_Payload_Is_Undefined() {
    var request = new ProcessPaymentRequest(PaymentMethod.Stripe, default);

    var result = _validator.TestValidate(request);

    result.ShouldHaveValidationErrorFor(x => x.Payload)
      .WithErrorMessage("Dữ liệu Webhook không được để trống.");
  }

  [Fact]
  public void Should_Have_Error_When_Payload_Is_Null() {
    var request = new ProcessPaymentRequest(PaymentMethod.Stripe, CreateNullPayload());

    var result = _validator.TestValidate(request);

    result.ShouldHaveValidationErrorFor(x => x.Payload)
      .WithErrorMessage("Dữ liệu Webhook không được để trống.");
  }

  [Fact]
  public void Should_Not_Have_Error_When_Request_Is_Valid() {
    var request = new ProcessPaymentRequest(PaymentMethod.Paypal, CreateValidPayload());

    var result = _validator.TestValidate(request);

    result.IsValid.Should().BeTrue();
  }

  private static JsonElement CreateValidPayload() {
    using var document = JsonDocument.Parse("{}");
    return document.RootElement.Clone();
  }

  private static JsonElement CreateNullPayload() {
    using var document = JsonDocument.Parse("null");
    return document.RootElement.Clone();
  }
}
