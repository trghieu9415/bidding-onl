using L2.Application.Filters;
using L2.Application.Models;
using L2.Application.UseCases.Bidders.Queries.GetLockedBidders;
using Tests.Unit.L2.Application.TestDoubles;
using Xunit;

namespace Tests.Unit.L2.Application.Bidders;

public class GetLockedBiddersHandlerTests {
  [Fact]
  public async Task Handle_ReturnsOnlyInactiveUsersAndMeta() {
    var activeUser = new User
      { Id = Guid.NewGuid(), FullName = "Active", Email = "a@example.com", IsActive = true, Role = UserRole.Bidder };
    var lockedUser = new User
      { Id = Guid.NewGuid(), FullName = "Locked", Email = "l@example.com", IsActive = false, Role = UserRole.Bidder };
    var userService = new StubUserService { UsersResult = (2, [activeUser, lockedUser]) };
    var filter = new UserFilter { Page = 1, PerPage = 10 };
    var handler = new GetLockedBiddersHandler(userService);

    var result = await handler.Handle(
      new GetLockedBiddersQuery(filter),
      TestContext.Current.CancellationToken
    );

    Assert.Equal([lockedUser], result.Bidders);
    Assert.Equal(Meta.Create(1, 10, 2), result.Meta);
    Assert.Equal(UserRole.Bidder, userService.LastRole);
  }
}
