using FluentAssertions;
using L2.Application.Abstractions;
using L2.Application.Behaviors;
using L2.Application.Ports.Messaging;
using MediatR;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace Tests.Unit.L2.Application.Behaviors;

public class TransactionBehaviorTests {
  private readonly TransactionBehavior<FakeTransactionalRequest, string> _behavior;
  private readonly IEventDispatcher _eventDispatcherMock;
  private readonly RequestHandlerDelegate<string> _nextMock;
  private readonly IUnitOfWork _unitOfWorkMock;

  public TransactionBehaviorTests() {
    _unitOfWorkMock = Substitute.For<IUnitOfWork>();
    _eventDispatcherMock = Substitute.For<IEventDispatcher>();
    _nextMock = Substitute.For<RequestHandlerDelegate<string>>();

    _behavior = new TransactionBehavior<FakeTransactionalRequest, string>(_unitOfWorkMock, _eventDispatcherMock);
  }

  [Fact]
  public async Task Handle_ShouldCommitTransaction_WhenNextSucceeds() {
    // Arrange
    var request = new FakeTransactionalRequest();
    const string expectedResponse = "SuccessResult";
    _nextMock.Invoke(TestContext.Current.CancellationToken).Returns(expectedResponse);

    var result = await _behavior.Handle(request, _nextMock, CancellationToken.None);
    result.Should().Be(expectedResponse);

    Received.InOrder(async void () => {
      await _unitOfWorkMock.BeginTransactionAsync(Arg.Any<CancellationToken>());
      await _nextMock.Invoke();
      await _eventDispatcherMock.DispatchEventsAsync(Arg.Any<CancellationToken>());
      await _unitOfWorkMock.SaveChangesAsync(Arg.Any<CancellationToken>());
      await _unitOfWorkMock.CommitTransactionAsync(Arg.Any<CancellationToken>());
    });

    await _unitOfWorkMock.DidNotReceive().RollbackTransactionAsync(Arg.Any<CancellationToken>());
  }

  [Fact]
  public async Task Handle_ShouldRollbackTransaction_WhenNextThrowsException() {
    var request = new FakeTransactionalRequest();
    var expectedException = new Exception("Handler failed!");
    _nextMock.Invoke(TestContext.Current.CancellationToken).Throws(expectedException);

    Func<Task> action = async () =>
      await _behavior.Handle(request, _nextMock, CancellationToken.None);

    await action.Should().ThrowAsync<Exception>().WithMessage("Handler failed!");

    await _unitOfWorkMock.Received(1).BeginTransactionAsync(Arg.Any<CancellationToken>());
    await _unitOfWorkMock.Received(1).RollbackTransactionAsync(Arg.Any<CancellationToken>());

    await _eventDispatcherMock.DidNotReceive().DispatchEventsAsync(Arg.Any<CancellationToken>());
    await _unitOfWorkMock.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    await _unitOfWorkMock.DidNotReceive().CommitTransactionAsync(Arg.Any<CancellationToken>());
  }
}

public class FakeTransactionalRequest : ITransactional, IRequest<string> {}
