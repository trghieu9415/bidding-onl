using FluentAssertions;
using L2.Application.Abstractions;
using L2.Application.Behaviors;
using L2.Application.Exceptions;
using L2.Application.Ports.Concurrency;
using MediatR;
using NSubstitute;
using Xunit;

namespace Tests.Unit.L2.Application.Behaviors;

public class LockBehaviorTests {
  private readonly LockBehavior<FakeLockableRequest, string> _behavior;
  private readonly IDistributedLockService _lockServiceMock;
  private readonly RequestHandlerDelegate<string> _nextMock;

  public LockBehaviorTests() {
    _lockServiceMock = Substitute.For<IDistributedLockService>();
    _nextMock = Substitute.For<RequestHandlerDelegate<string>>();
    _behavior = new LockBehavior<FakeLockableRequest, string>(_lockServiceMock);
  }

  [Fact]
  public async Task Handle_ShouldCallNextAndDisposeLock_WhenLockIsAcquiredSuccessfully() {
    var request = new FakeLockableRequest {
      LockKey = "auction-123",
      WaitTime = TimeSpan.FromSeconds(3)
    };

    const string expectedResponse = "SuccessResult";
    var mockDisposableLock = Substitute.For<IDisposable>();

    _lockServiceMock
      .AcquireLockAsync(request.LockKey, request.WaitTime)
      .Returns(mockDisposableLock);

    _nextMock.Invoke(TestContext.Current.CancellationToken).Returns(expectedResponse);
    var result = await _behavior.Handle(request, _nextMock, CancellationToken.None);
    result.Should().Be(expectedResponse);

    await _nextMock.Received(1).Invoke(TestContext.Current.CancellationToken);

    mockDisposableLock.Received(1).Dispose();
  }

  [Fact]
  public async Task Handle_ShouldThrowWorkflowException_WhenLockAcquisitionFails() {
    var request = new FakeLockableRequest {
      LockKey = "auction-456",
      WaitTime = TimeSpan.FromSeconds(3)
    };

    _lockServiceMock
      .AcquireLockAsync(request.LockKey, request.WaitTime)
      .Returns((IDisposable?)null);

    Func<Task> action = async () => await _behavior.Handle(request, _nextMock, CancellationToken.None);
    var exception = await action.Should().ThrowAsync<WorkflowException>();

    exception.Which.StatusCode.Should().Be(429);
    exception.Which.Message.Should().Be("Hệ thống đang bận xử lý yêu cầu này. Vui lòng thử lại.");

    await _nextMock.DidNotReceive().Invoke(TestContext.Current.CancellationToken);
  }
}

public class FakeLockableRequest : ILockable, IRequest<string> {
  public string LockKey { get; init; } = string.Empty;
  public TimeSpan WaitTime { get; init; }
}
