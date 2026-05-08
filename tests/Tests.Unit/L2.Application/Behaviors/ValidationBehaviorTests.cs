using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using L2.Application.Behaviors;
using L2.Application.Exceptions;
using MediatR;
using NSubstitute;
using Xunit;

namespace Tests.Unit.L2.Application.Behaviors;

#pragma warning disable xUnit1051
public class ValidationBehaviorTests {
  [Fact]
  public async Task Handle_Should_InvokeNext_WhenNoValidatorsExist() {
    // Arrange
    var validators = Enumerable.Empty<IValidator<TestValidationRequest>>();
    var sut = new ValidationBehavior<TestValidationRequest, string>(validators);
    var request = new TestValidationRequest();

    var next = Substitute.For<RequestHandlerDelegate<string>>();
    next.Invoke().Returns("success");

    // Act
    var result = await sut.Handle(request, next, CancellationToken.None);

    // Assert
    result.Should().Be("success");
  }

  [Fact]
  public async Task Handle_Should_InvokeNext_WhenValidationPasses() {
    // Arrange
    var validator = Substitute.For<IValidator<TestValidationRequest>>();

    validator
      .ValidateAsync(Arg.Any<IValidationContext>(), Arg.Any<CancellationToken>())
      .Returns(new ValidationResult());

    var sut = new ValidationBehavior<TestValidationRequest, string>([validator]);
    var request = new TestValidationRequest();

    var next = Substitute.For<RequestHandlerDelegate<string>>();
    next.Invoke().Returns("success");

    // Act
    var result = await sut.Handle(request, next, CancellationToken.None);

    // Assert
    result.Should().Be("success");
  }

  [Fact]
  public async Task Handle_Should_ThrowInvalidInputException_WhenValidationFails() {
    // Arrange
    var validator1 = Substitute.For<IValidator<TestValidationRequest>>();
    var validator2 = Substitute.For<IValidator<TestValidationRequest>>();

    validator1
      .ValidateAsync(Arg.Any<IValidationContext>(), Arg.Any<CancellationToken>())
      .Returns(new ValidationResult([
        new ValidationFailure("Property1", "Lỗi số 1"),
        new ValidationFailure("Property2", "Lỗi số 2")
      ]));

    validator2
      .ValidateAsync(Arg.Any<IValidationContext>(), Arg.Any<CancellationToken>())
      .Returns(new ValidationResult([
        new ValidationFailure("Property1", "Lỗi số 1"),
        new ValidationFailure("Property3", "Lỗi số 3")
      ]));

    var sut = new ValidationBehavior<TestValidationRequest, string>([validator1, validator2]);
    var request = new TestValidationRequest();

    var next = Substitute.For<RequestHandlerDelegate<string>>();
    next.Invoke().Returns("success");

    // Act
    var act = async () => await sut.Handle(request, next, CancellationToken.None);

    // Assert
    var exception = await act.Should().ThrowAsync<InvalidInputException>();
    exception.Which.Message.Should().Be("Đầu vào không hợp lệ");
    exception.Which.Errors.Should().BeEquivalentTo(
      "Lỗi số 1", "Lỗi số 2", "Lỗi số 3"
    );
  }

  public sealed class TestValidationRequest : IRequest<string>;
}
