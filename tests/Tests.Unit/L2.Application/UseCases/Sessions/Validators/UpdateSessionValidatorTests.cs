using FluentAssertions;
using FluentValidation.TestHelper;
using L2.Application.UseCases.Sessions.Commands.UpdateSession;
using Xunit;

namespace Tests.Unit.L2.Application.UseCases.Sessions.Validators;

public class UpdateSessionValidatorTests {
  private readonly UpdateSessionValidator _validator = new();

  [Theory]
  [InlineData("")]
  [InlineData(" ")]
  public void Should_Have_Error_When_Title_Is_Empty(string title) {
    var request = new UpdateSessionRequest(title, DateTime.UtcNow.AddMinutes(5), DateTime.UtcNow.AddMinutes(10));

    var result = _validator.TestValidate(request);

    result.ShouldHaveValidationErrorFor(x => x.Title)
      .WithErrorMessage("Tiêu đề phiên đấu giá không được để trống.");
  }

  [Fact]
  public void Should_Have_Error_When_Title_Exceeds_MaxLength() {
    var request = new UpdateSessionRequest(new string('a', 201), DateTime.UtcNow.AddMinutes(5), DateTime.UtcNow.AddMinutes(10));

    var result = _validator.TestValidate(request);

    result.ShouldHaveValidationErrorFor(x => x.Title)
      .WithErrorMessage("Tiêu đề phiên đấu giá không được vượt quá 200 ký tự.");
  }

  [Fact]
  public void Should_Have_Error_When_EndTime_Is_Not_Greater_Than_StartTime() {
    var startTime = DateTime.UtcNow.AddMinutes(5);
    var request = new UpdateSessionRequest("Phiên đấu giá tháng 5", startTime, startTime);

    var result = _validator.TestValidate(request);

    result.ShouldHaveValidationErrorFor(x => x.EndTime)
      .WithErrorMessage("Thời gian kết thúc phải lớn hơn thời gian bắt đầu.");
  }

  [Fact]
  public void Should_Not_Have_Error_When_Request_Is_Valid() {
    var request = new UpdateSessionRequest("Phiên đấu giá tháng 5", DateTime.UtcNow.AddMinutes(5), DateTime.UtcNow.AddMinutes(10));

    var result = _validator.TestValidate(request);

    result.IsValid.Should().BeTrue();
  }
}
