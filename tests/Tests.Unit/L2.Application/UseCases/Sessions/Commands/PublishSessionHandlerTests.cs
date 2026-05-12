using System.Runtime.Serialization;
using FluentAssertions;
using L1.Core.Domain.Bidding.Entities;
using L1.Core.Domain.Bidding.Enums;
using L2.Application.Exceptions;
using L2.Application.Repositories;
using L2.Application.UseCases.Sessions.Commands.PublishSession;
using NSubstitute;
using Tests.Common.Builders;
using Xunit;

namespace Tests.Unit.L2.Application.UseCases.Sessions.Commands;

#pragma warning disable SYSLIB0050
public class PublishSessionHandlerTests {
  private readonly IRepository<AuctionSession> _repository = Substitute.For<IRepository<AuctionSession>>();
  private readonly PublishSessionHandler _sut;

  public PublishSessionHandlerTests() {
    _sut = new PublishSessionHandler(_repository);
  }

  [Fact]
  public async Task Handle_Should_ThrowWorkflowException_When_SessionNotFound() {
    var request = new PublishSessionCommand(Guid.NewGuid());

    _repository.GetByIdAsync(request.Id, CancellationToken.None).Returns((AuctionSession?)null);

    var act = async () => await _sut.Handle(request, CancellationToken.None);

    var exception = await act.Should().ThrowAsync<WorkflowException>();
    exception.Which.StatusCode.Should().Be(404);
    exception.Which.Message.Should().Be("Không tìm thấy phiên");
  }

  [Fact]
  public async Task Handle_Should_ThrowWorkflowException_When_TimeFrameIsMissing() {
    var session = (AuctionSession)FormatterServices.GetUninitializedObject(typeof(AuctionSession));
    var request = new PublishSessionCommand(Guid.NewGuid());

    _repository.GetByIdAsync(request.Id, CancellationToken.None).Returns(session);

    var act = async () => await _sut.Handle(request, CancellationToken.None);

    var exception = await act.Should().ThrowAsync<WorkflowException>();
    exception.Which.Message.Should().Be("Phải thiết lập thời gian trước khi công khai phiên.");
  }

  [Fact]
  public async Task Handle_Should_PublishSession_And_ReturnTrue() {
    var session = new AuctionSessionBuilder().Build();
    var request = new PublishSessionCommand(session.Id);

    _repository.GetByIdAsync(request.Id, CancellationToken.None).Returns(session);

    var result = await _sut.Handle(request, CancellationToken.None);

    result.Should().BeTrue();
    session.Status.Should().Be(SessionStatus.Published);
    await _repository.Received(1).UpdateAsync(session, CancellationToken.None);
  }
}
#pragma warning restore SYSLIB0050
