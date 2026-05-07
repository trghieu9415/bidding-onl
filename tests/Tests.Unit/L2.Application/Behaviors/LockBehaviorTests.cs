using FluentAssertions;
using L2.Application.Abstractions;
using L2.Application.Behaviors;
using L2.Application.Exceptions;
using L2.Application.Ports.Concurrency;
using NSubstitute;
using Xunit;

namespace Tests.Unit.L2.Application.Behaviors;

public class LockBehaviorTests {
  private readonly IDistributedLockService _lockService = Substitute.For<IDistributedLockService>();

  private readonly LockBehavior<TestLockRequest, string> _sut;

  public LockBehaviorTests() {
    _sut = new LockBehavior<TestLockRequest, string>(_lockService);
  }

  [Fact]
  public async Task Handle_Should_InvokeNext_WhenLockAcquired() {
    // Arrange
    var request = new TestLockRequest();

    var distributedLock = Substitute.For<IDisposable>();

    _lockService
      .AcquireLockAsync(request.LockKey, request.WaitTime)
      .Returns(distributedLock);

    // Act
    var result = await _sut.Handle(
      request,
      Next,
      CancellationToken.None
    );

    // Assert
    result.Should().Be("success");

    await _lockService
      .Received(1)
      .AcquireLockAsync(request.LockKey, request.WaitTime);

    distributedLock
      .Received(1)
      .Dispose();

    return;

    Task<string> Next(CancellationToken _) {
      return Task.FromResult("success");
    }
  }

  [Fact]
  public async Task Handle_Should_ThrowWorkflowException_WhenLockNotAcquired() {
    // Arrange
    var request = new TestLockRequest();

    _lockService
      .AcquireLockAsync(request.LockKey, request.WaitTime)
      .Returns((IDisposable?)null);

    // Act
    var act = async () => await _sut.Handle(
      request,
      Next,
      CancellationToken.None
    );

    // Assert
    var exception = await act.Should()
      .ThrowAsync<WorkflowException>();

    exception.Which.StatusCode.Should().Be(429);

    exception.Which.Message.Should()
      .Be("Hệ thống đang bận xử lý yêu cầu này. Vui lòng thử lại.");

    await _lockService
      .Received(1)
      .AcquireLockAsync(request.LockKey, request.WaitTime);
    return;

    Task<string> Next(CancellationToken _) {
      return Task.FromResult("success");
    }
  }

  public sealed class TestLockRequest : ILockable {
    public string LockKey => "test-lock";

    public TimeSpan WaitTime => TimeSpan.FromSeconds(5);
  }
}
