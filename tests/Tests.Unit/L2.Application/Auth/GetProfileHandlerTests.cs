using L2.Application.Exceptions;
using L2.Application.Models;
using L2.Application.UseCases.Auth.Queries.GetProfile;
using Tests.Unit.L2.Application.TestDoubles;
using Xunit;

namespace Tests.Unit.L2.Application.Auth;

public class GetProfileHandlerTests {
  [Fact]
  public async Task Handle_WhenMissing_ThrowsWorkflowException() {
    var handler = new GetProfileHandler(new StubUserService());

    var exception = await Assert.ThrowsAsync<WorkflowException>(async () =>
      await handler.Handle(new GetProfileQuery(Guid.NewGuid(), UserRole.Bidder), TestContext.Current.CancellationToken));

    Assert.Equal(404, exception.StatusCode);
    Assert.Equal("Người dùng không tồn tại", exception.Message);
  }
}
