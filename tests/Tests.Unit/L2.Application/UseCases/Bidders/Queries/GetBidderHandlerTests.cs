using FluentAssertions;
using L2.Application.Exceptions;
using L2.Application.Models;
using L2.Application.Ports.Security;
using L2.Application.UseCases.Bidders.Queries.GetBidder;
using NSubstitute;
using Tests.Unit.L2.Application.UseCases;
using Xunit;

namespace Tests.Unit.L2.Application.UseCases.Bidders.Queries;

public class GetBidderHandlerTests {
  private readonly IUserService _userService = Substitute.For<IUserService>();
  private readonly GetBidderHandler _sut;

  public GetBidderHandlerTests() {
    _sut = new GetBidderHandler(_userService);
  }

  [Fact]
  public async Task Handle_Should_ThrowWorkflowException_When_BidderNotFound() {
    var request = new GetBidderQuery(Guid.NewGuid());

    _userService.GetByIdAsync(request.Id, UserRole.Bidder, CancellationToken.None)
      .Returns((User?)null);

    var act = async () => await _sut.Handle(request, CancellationToken.None);

    var exception = await act.Should().ThrowAsync<WorkflowException>();
    exception.Which.StatusCode.Should().Be(404);
    exception.Which.Message.Should().Be("Không tìm thấy người dùng");
  }

  [Fact]
  public async Task Handle_Should_ReturnBidder_When_Found() {
    var bidder = UseCaseTestData.CreateUser(role: UserRole.Bidder);
    var request = new GetBidderQuery(bidder.Id);

    _userService.GetByIdAsync(request.Id, UserRole.Bidder, CancellationToken.None)
      .Returns(bidder);

    var result = await _sut.Handle(request, CancellationToken.None);

    result.Bidder.Should().Be(bidder);
  }
}
