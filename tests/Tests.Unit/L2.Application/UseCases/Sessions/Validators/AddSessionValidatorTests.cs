using FluentAssertions;
using FluentValidation.TestHelper;
using L2.Application.UseCases.Sessions.Commands.AddSession;
using Xunit;

namespace Tests.Unit.L2.Application.UseCases.Sessions.Validators;

public class AddSessionValidatorTests {
  private readonly AddSessionValidator _validator = new();

  [Theory]
  [InlineData("")]
  [InlineData(" ")]
  public void Should_Have_Error_When_Title_Is_Empty(string title) {
    var command = CreateValidCommand() with { Title = title };

    var result = _validator.TestValidate(command);

    result.ShouldHaveValidationErrorFor(x => x.Title)
      .WithErrorMessage("Tiêu đề phiên đấu giá không được để trống.");
  }

  [Fact]
  public void Should_Have_Error_When_Title_Exceeds_MaxLength() {
    var command = CreateValidCommand() with { Title = new string('a', 201) };

    var result = _validator.TestValidate(command);

    result.ShouldHaveValidationErrorFor(x => x.Title)
      .WithErrorMessage("Tiêu đề phiên đấu giá không được vượt quá 200 ký tự.");
  }

  [Fact]
  public void Should_Have_Error_When_StartTime_Is_In_The_Past() {
    var command = CreateValidCommand() with { StartTime = DateTime.UtcNow.AddMinutes(-5) };

    var result = _validator.TestValidate(command);

    result.ShouldHaveValidationErrorFor(x => x.StartTime)
      .WithErrorMessage("Thời gian bắt đầu phải lớn hơn thời gian hiện tại.");
  }

  [Fact]
  public void Should_Have_Error_When_EndTime_Is_Not_Greater_Than_StartTime() {
    var startTime = DateTime.UtcNow.AddMinutes(5);
    var command = CreateValidCommand() with { StartTime = startTime, EndTime = startTime };

    var result = _validator.TestValidate(command);

    result.ShouldHaveValidationErrorFor(x => x.EndTime)
      .WithErrorMessage("Thời gian kết thúc phải lớn hơn thời gian bắt đầu.");
  }

  [Fact]
  public void Should_Have_Error_When_AuctionIds_Is_Null() {
    var command = CreateValidCommand() with { AuctionIds = null! };

    var result = _validator.TestValidate(command);

    result.ShouldHaveValidationErrorFor(x => x.AuctionIds)
      .WithErrorMessage("Danh sách phiên đấu giá không được để trống.");
  }

  [Fact]
  public void Should_Have_Error_When_AuctionIds_Contains_Empty_Id() {
    var command = CreateValidCommand() with { AuctionIds = [Guid.Empty] };

    var result = _validator.TestValidate(command);

    result.ShouldHaveValidationErrorFor("AuctionIds[0]")
      .WithErrorMessage("Id phiên đấu giá không hợp lệ.");
  }

  [Fact]
  public void Should_Not_Have_Error_When_Command_Is_Valid() {
    var command = CreateValidCommand();

    var result = _validator.TestValidate(command);

    result.IsValid.Should().BeTrue();
  }

  private static AddSessionCommand CreateValidCommand() {
    return new AddSessionCommand(
      "Phiên đấu giá tháng 5",
      DateTime.UtcNow.AddMinutes(5),
      DateTime.UtcNow.AddMinutes(10),
      [Guid.NewGuid()]
    );
  }
}
