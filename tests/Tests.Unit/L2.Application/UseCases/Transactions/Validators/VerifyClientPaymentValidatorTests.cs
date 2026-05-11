using System.Text.Json;
using FluentAssertions;
using FluentValidation.TestHelper;
using L2.Application.UseCases.Transactions.Commands.VerifyPayment;
using Xunit;

namespace Tests.Unit.L2.Application.UseCases.Transactions.Validators;

public class VerifyClientPaymentValidatorTests {
  private readonly VerifyClientPaymentValidator _validator = new();

  [Fact]
  public void Should_Have_Error_When_Id_Is_Empty() {
    var request = new VerifyClientPaymentRequest(Guid.Empty, CreateValidPayload());

    var result = _validator.TestValidate(request);

    result.ShouldHaveValidationErrorFor(x => x.Id)
      .WithErrorMessage("Id thanh toán không được để trống.");
  }

  [Fact]
  public void Should_Have_Error_When_Payload_Is_Undefined() {
    var request = new VerifyClientPaymentRequest(Guid.NewGuid(), default);

    var result = _validator.TestValidate(request);

    result.ShouldHaveValidationErrorFor(x => x.Payload)
      .WithErrorMessage("Dữ liệu thanh toán không được để trống.");
  }

  [Fact]
  public void Should_Have_Error_When_Payload_Is_Null() {
    var request = new VerifyClientPaymentRequest(Guid.NewGuid(), CreateNullPayload());

    var result = _validator.TestValidate(request);

    result.ShouldHaveValidationErrorFor(x => x.Payload)
      .WithErrorMessage("Dữ liệu thanh toán không được để trống.");
  }

  [Fact]
  public void Should_Not_Have_Error_When_Request_Is_Valid() {
    var request = new VerifyClientPaymentRequest(Guid.NewGuid(), CreateValidPayload());

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
