using FluentAssertions;
using FluentValidation.TestHelper;
using L2.Application.UseCases.Bids.Commands.PlaceBid;
using Xunit;

namespace Tests.Unit.L2.Application.UseCases.Bids.Validators;

public class PlaceBidValidatorTests {
  private readonly PlaceBidValidator _validator = new();

  [Fact]
  public void Should_Have_Error_When_AuctionId_Is_Empty() {
    var command = new PlaceBidCommand(Guid.Empty, Guid.NewGuid(), "Bidder", new PlaceBidRequest(1000));

    var result = _validator.TestValidate(command);

    result.ShouldHaveValidationErrorFor(x => x.AuctionId)
      .WithErrorMessage("Id phiên đấu giá không được để trống.");
  }

  [Fact]
  public void Should_Have_Error_When_UserId_Is_Empty() {
    var command = new PlaceBidCommand(Guid.NewGuid(), Guid.Empty, "Bidder", new PlaceBidRequest(1000));

    var result = _validator.TestValidate(command);

    result.ShouldHaveValidationErrorFor(x => x.UserId)
      .WithErrorMessage("Id người dùng không được để trống.");
  }

  [Theory]
  [InlineData("")]
  [InlineData(" ")]
  public void Should_Have_Error_When_UserFullName_Is_Empty(string fullName) {
    var command = new PlaceBidCommand(Guid.NewGuid(), Guid.NewGuid(), fullName, new PlaceBidRequest(1000));

    var result = _validator.TestValidate(command);

    result.ShouldHaveValidationErrorFor(x => x.UserFullName)
      .WithErrorMessage("Tên người đấu giá không được để trống.");
  }

  [Fact]
  public void Should_Have_Error_When_UserFullName_Exceeds_MaxLength() {
    var command = new PlaceBidCommand(Guid.NewGuid(), Guid.NewGuid(), new string('a', 201), new PlaceBidRequest(1000));

    var result = _validator.TestValidate(command);

    result.ShouldHaveValidationErrorFor(x => x.UserFullName)
      .WithErrorMessage("Tên người đấu giá không được vượt quá 200 ký tự.");
  }

  [Fact]
  public void Should_Have_Error_When_Data_Is_Null() {
    var command = new PlaceBidCommand(Guid.NewGuid(), Guid.NewGuid(), "Bidder", null!);

    var result = _validator.TestValidate(command);

    result.ShouldHaveValidationErrorFor(x => x.Data)
      .WithErrorMessage("Thông tin đấu giá không được để trống.");
  }

  [Fact]
  public void Should_Not_Have_Error_When_Command_Is_Valid() {
    var command = new PlaceBidCommand(Guid.NewGuid(), Guid.NewGuid(), "Nguyen Van A", new PlaceBidRequest(1000));

    var result = _validator.TestValidate(command);

    result.IsValid.Should().BeTrue();
  }
}
