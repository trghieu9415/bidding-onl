using FluentAssertions;
using FluentValidation.TestHelper;
using L1.Core.Domain.Transaction.ValueObjects;
using L2.Application.UseCases.Transactions.Commands.CreateOrder;
using Xunit;

namespace Tests.Unit.L2.Application.UseCases.Transactions.Validators;

public class CreateOrderValidatorTests {
  private readonly CreateOrderValidator _validator = new();

  [Fact]
  public void Should_Have_Error_When_AuctionId_Is_Empty() {
    var request = new CreateOrderRequest(Guid.Empty, CreateValidAddress());

    var result = _validator.TestValidate(request);

    result.ShouldHaveValidationErrorFor(x => x.AuctionId)
      .WithErrorMessage("Id phiên đấu giá không được để trống.");
  }

  [Fact]
  public void Should_Have_Error_When_Address_Is_Null() {
    var request = new CreateOrderRequest(Guid.NewGuid(), null!);

    var result = _validator.TestValidate(request);

    result.ShouldHaveValidationErrorFor(x => x.Address)
      .WithErrorMessage("Địa chỉ giao hàng không được để trống.");
  }

  [Fact]
  public void Should_Not_Have_Error_When_Request_Is_Valid() {
    var request = new CreateOrderRequest(Guid.NewGuid(), CreateValidAddress());

    var result = _validator.TestValidate(request);

    result.IsValid.Should().BeTrue();
  }

  private static Address CreateValidAddress() {
    return new Address("Nguyen Van A", "0912345678", "123 Nguyen Trai, Quan 1");
  }
}
