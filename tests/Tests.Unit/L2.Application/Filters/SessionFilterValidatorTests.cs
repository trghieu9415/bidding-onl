using FluentValidation.TestHelper;
using L2.Application.Filters;
using Xunit;

namespace Tests.Unit.L2.Application.Filters;

public class SessionFilterValidatorTests {
  private readonly SessionFilterValidator _validator = new();

  [Fact]
  public void Should_Have_Error_When_StartTime_Is_Greater_Than_EndTime() {
    // Arrange
    var filter = new SessionFilter {
      StartTime = DateTime.Now.AddDays(1),
      EndTime = DateTime.Now
    };

    // Act
    var result = _validator.TestValidate(filter);

    // Assert
    result.ShouldHaveValidationErrors()
      .WithErrorMessage("Thời gian bắt đầu không được lớn hơn thời gian kết thúc.");
  }

  [Fact]
  public void Should_Have_Error_When_Title_Exceeds_MaxLength() {
    // Arrange
    var filter = new SessionFilter { Title = new string('a', 201) };

    // Act
    var result = _validator.TestValidate(filter);

    // Assert
    result.ShouldHaveValidationErrorFor(x => x.Title);
  }
}
