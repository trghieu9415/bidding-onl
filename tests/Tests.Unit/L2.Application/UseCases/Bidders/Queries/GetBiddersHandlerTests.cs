using FluentAssertions;
using L2.Application.Filters;
using L2.Application.Models;
using L2.Application.Ports.Security;
using L2.Application.UseCases.Bidders.Queries.GetBidders;
using NSubstitute;
using Tests.Unit.L2.Application.UseCases;
using Xunit;

namespace Tests.Unit.L2.Application.UseCases.Bidders.Queries;

public class GetBiddersHandlerTests {
  private readonly IUserService _userService = Substitute.For<IUserService>();
  private readonly GetBiddersHandler _sut;

  public GetBiddersHandlerTests() {
    _sut = new GetBiddersHandler(_userService);
  }

  [Fact]
  public async Task Handle_Should_ReturnBidders_And_Meta() {
    var filter = new UserFilter { Page = 1, PerPage = 10 };
    var bidders = new List<User> {
      UseCaseTestData.CreateUser(role: UserRole.Bidder),
      UseCaseTestData.CreateUser(role: UserRole.Bidder)
    };
    var request = new GetBiddersQuery(filter);

    _userService.GetAsync(filter, UserRole.Bidder, CancellationToken.None)
      .Returns((25, bidders));

    var result = await _sut.Handle(request, CancellationToken.None);

    result.Bidders.Should().BeSameAs(bidders);
    result.Meta.Total.Should().Be(25);
    result.Meta.TotalPages.Should().Be(3);
  }
}
