using L2.Application.UseCases.Bidders.Commands.LockBidder;
using Tests.Unit.L2.Application.TestDoubles;
using Xunit;

namespace Tests.Unit.L2.Application.Bidders;

public class LockBidderHandlerTests {
  [Fact]
  public async Task Handle_CallsUserServiceAndReturnsTrue() {
    var userService = new StubUserService();
    var handler = new LockBidderHandler(userService);
    var userId = Guid.NewGuid();

    var result = await handler.Handle(new LockBidderCommand(userId), TestContext.Current.CancellationToken);

    Assert.True(result);
    Assert.Equal(userId, userService.LockedId);
  }
}
