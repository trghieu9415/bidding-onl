using FluentAssertions;
using L2.Application.Filters;
using L2.Application.Models;
using L2.Application.Ports.Security;
using L2.Application.UseCases.Bidders.Queries.GetLockedBidders;
using NSubstitute;
using Tests.Unit.L2.Application.UseCases;
using Xunit;

namespace Tests.Unit.L2.Application.UseCases.Bidders.Queries;

public class GetLockedBiddersHandlerTests {
  private readonly IUserService _userService = Substitute.For<IUserService>();
  private readonly GetLockedBiddersHandler _sut;

  public GetLockedBiddersHandlerTests() {
    _sut = new GetLockedBiddersHandler(_userService);
  }

  [Fact]
  public async Task Handle_Should_Filter_Inactive_Bidders_And_ReturnMeta() {
    var filter = new UserFilter { Page = 1, PerPage = 10 };
    var users = new List<User> {
      UseCaseTestData.CreateUser(isActive: false),
      UseCaseTestData.CreateUser(isActive: true),
      UseCaseTestData.CreateUser(isActive: false)
    };
    var request = new GetLockedBiddersQuery(filter);

    _userService.GetAsync(filter, UserRole.Bidder, CancellationToken.None)
      .Returns((20, users));

    var result = await _sut.Handle(request, CancellationToken.None);

    result.Bidders.Should().HaveCount(2);
    result.Bidders.Should().OnlyContain(x => !x.IsActive);
    result.Meta.Total.Should().Be(20);
  }
}
