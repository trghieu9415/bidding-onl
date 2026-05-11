using FluentAssertions;
using FluentValidation.TestHelper;
using L2.Application.UseCases.Sessions.Commands.SyncAuctions;
using Xunit;

namespace Tests.Unit.L2.Application.UseCases.Sessions.Validators;

public class SyncAuctionsValidatorTests {
  private readonly SyncAuctionsValidator _validator = new();

  [Fact]
  public void Should_Have_Error_When_Id_Is_Empty() {
    var command = new SyncAuctionsCommand(Guid.Empty, [Guid.NewGuid()]);

    var result = _validator.TestValidate(command);

    result.ShouldHaveValidationErrorFor(x => x.Id)
      .WithErrorMessage("Id phiên đấu giá không được để trống.");
  }

  [Fact]
  public void Should_Have_Error_When_AuctionIds_Is_Null() {
    var command = new SyncAuctionsCommand(Guid.NewGuid(), null!);

    var result = _validator.TestValidate(command);

    result.ShouldHaveValidationErrorFor(x => x.AuctionIds)
      .WithErrorMessage("Danh sách phiên đấu giá không được để trống.");
  }

  [Fact]
  public void Should_Have_Error_When_AuctionIds_Contains_Empty_Id() {
    var command = new SyncAuctionsCommand(Guid.NewGuid(), [Guid.Empty]);

    var result = _validator.TestValidate(command);

    result.ShouldHaveValidationErrorFor("AuctionIds[0]")
      .WithErrorMessage("Id phiên đấu giá không hợp lệ.");
  }

  [Fact]
  public void Should_Not_Have_Error_When_Command_Is_Valid() {
    var command = new SyncAuctionsCommand(Guid.NewGuid(), [Guid.NewGuid()]);

    var result = _validator.TestValidate(command);

    result.IsValid.Should().BeTrue();
  }
}
