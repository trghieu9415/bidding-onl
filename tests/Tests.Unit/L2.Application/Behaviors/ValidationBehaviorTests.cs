using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using L2.Application.Behaviors;
using L2.Application.Exceptions;
using MediatR;
using NSubstitute;
using Xunit;

namespace Tests.Unit.L2.Application.Behaviors;

public class ValidationBehaviorTests {
  private readonly RequestHandlerDelegate<string> _nextMock;

  public ValidationBehaviorTests() {
    _nextMock = Substitute.For<RequestHandlerDelegate<string>>();
    _nextMock.Invoke().Returns("NextCalled");
  }

  [Fact]
  public async Task Handle_ShouldCallNext_WhenNoValidatorsExist() {
    // Arrange: Không tiêm validator nào vào
    var behavior = new ValidationBehavior<FakeRequest, string>(Enumerable.Empty<IValidator<FakeRequest>>());
    var request = new FakeRequest();

    // Act
    var result = await behavior.Handle(request, _nextMock, CancellationToken.None);

    // Assert
    result.Should().Be("NextCalled");
    await _nextMock.Received(1).Invoke(TestContext.Current.CancellationToken);
  }

  [Fact]
  public async Task Handle_ShouldCallNext_WhenValidationPasses() {
    // Arrange: Có validator nhưng dữ liệu hợp lệ (không sinh ra lỗi)
    var validatorMock = Substitute.For<IValidator<FakeRequest>>();
    validatorMock.ValidateAsync(Arg.Any<ValidationContext<FakeRequest>>(), Arg.Any<CancellationToken>())
      .Returns(new ValidationResult()); // Kết quả rỗng = Hợp lệ

    var behavior = new ValidationBehavior<FakeRequest, string>(new[] { validatorMock });
    var request = new FakeRequest();

    // Act
    var result = await behavior.Handle(request, _nextMock, CancellationToken.None);

    // Assert
    result.Should().Be("NextCalled");
    await _nextMock.Received(1).Invoke(TestContext.Current.CancellationToken);
  }

  [Fact]
  public async Task Handle_ShouldThrowInvalidInputException_WithDistinctMessages_WhenValidationFails() {
    var validatorMock1 = Substitute.For<IValidator<FakeRequest>>();
    var validatorMock2 = Substitute.For<IValidator<FakeRequest>>();

    validatorMock1.ValidateAsync(Arg.Any<ValidationContext<FakeRequest>>(), Arg.Any<CancellationToken>())
      .Returns(new ValidationResult(new[] {
        new ValidationFailure("Prop1", "Lỗi số 1"),
        new ValidationFailure("Prop1", "Lỗi số 1")
      }));

    validatorMock2.ValidateAsync(Arg.Any<ValidationContext<FakeRequest>>(), Arg.Any<CancellationToken>())
      .Returns(new ValidationResult([
        new ValidationFailure("Prop2",
          "Lỗi số 2"
        )
      ]));

    var behavior = new ValidationBehavior<FakeRequest, string>(new[] { validatorMock1, validatorMock2 });
    var request = new FakeRequest();

    Func<Task> action = async () => await behavior.Handle(request, _nextMock, CancellationToken.None);

    var exception = await action.Should().ThrowAsync<InvalidInputException>();

    exception.Which.Message.Should().Contain("Lỗi số 1");
    exception.Which.Message.Should().Contain("Lỗi số 2");

    await _nextMock.DidNotReceive().Invoke(TestContext.Current.CancellationToken);
  }
}

public class FakeRequest : IRequest<string> {}
