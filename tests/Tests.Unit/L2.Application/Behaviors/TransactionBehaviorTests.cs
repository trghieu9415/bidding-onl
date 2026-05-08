using FluentAssertions;
using L2.Application.Abstractions;
using L2.Application.Behaviors;
using L2.Application.Ports.Messaging;
using MediatR;
using NSubstitute;
using Xunit;

namespace Tests.Unit.L2.Application.Behaviors;

#pragma warning disable xUnit1051
public class TransactionBehaviorTests {
  private readonly IEventDispatcher _eventDispatcher = Substitute.For<IEventDispatcher>();
  private readonly TransactionBehavior<TestTransactionRequest, string> _sut;
  private readonly ITransactionMock _transaction = Substitute.For<ITransactionMock>();
  private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();

  public TransactionBehaviorTests() {
    _unitOfWork
      .BeginTransactionAsync(Arg.Any<CancellationToken>())
      .Returns(_transaction);

    _sut = new TransactionBehavior<TestTransactionRequest, string>(_unitOfWork, _eventDispatcher);
  }

  [Fact]
  public async Task Handle_Should_CommitAndDispose_WhenSuccess() {
    // Arrange
    var request = new TestTransactionRequest();
    var transaction = Substitute.For<IAsyncDisposable>();

    _unitOfWork
      .BeginTransactionAsync(Arg.Any<CancellationToken>())
      .Returns(transaction);

    var next = Substitute.For<RequestHandlerDelegate<string>>();
    next.Invoke().Returns("success");

    // Act
    var result = await _sut.Handle(request, next, CancellationToken.None);

    // Assert
    result.Should().Be("success");

    await _unitOfWork.Received(1).BeginTransactionAsync(CancellationToken.None);
    await next.Received(1).Invoke();
    await _eventDispatcher.Received(1).DispatchEventsAsync(CancellationToken.None);
    await _unitOfWork.Received(1).SaveChangesAsync(CancellationToken.None);
    await _unitOfWork.Received(1).CommitTransactionAsync(CancellationToken.None);
    await transaction.Received(1).DisposeAsync();
  }

  [Fact]
  public async Task Handle_Should_RollbackImplicitly_WhenNextThrowsException() {
    // Arrange
    var request = new TestTransactionRequest();
    var expectedException = new InvalidOperationException("Test exception");
    var transaction = Substitute.For<IAsyncDisposable>();
    _unitOfWork
      .BeginTransactionAsync(Arg.Any<CancellationToken>())
      .Returns(transaction);

    var next = Substitute.For<RequestHandlerDelegate<string>>();
    next.Invoke().Returns(Task.FromException<string>(expectedException));

    // Act
    var act = async () => await _sut.Handle(request, next, CancellationToken.None);

    // Assert
    await act.Should().ThrowAsync<InvalidOperationException>()
      .WithMessage(expectedException.Message);

    await _unitOfWork.Received(1).BeginTransactionAsync(CancellationToken.None);
    await _eventDispatcher.DidNotReceive().DispatchEventsAsync(Arg.Any<CancellationToken>());
    await _unitOfWork.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    await _unitOfWork.DidNotReceive().CommitTransactionAsync(Arg.Any<CancellationToken>());
    await transaction.Received(1).DisposeAsync();
  }

  public interface ITransactionMock : IAsyncDisposable;

  public sealed class TestTransactionRequest : ITransactional;
}
