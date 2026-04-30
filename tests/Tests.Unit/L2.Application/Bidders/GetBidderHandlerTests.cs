using L2.Application.Exceptions;
using L2.Application.UseCases.Bidders.Queries.GetBidder;
using Tests.Unit.L2.Application.TestDoubles;
using Xunit;

namespace Tests.Unit.L2.Application.Bidders;

public class GetBidderHandlerTests {
  [Fact]
  public async Task Handle_WhenMissing_ThrowsWorkflowException() {
    var handler = new GetBidderHandler(new StubUserService());

    var exception = await Assert.ThrowsAsync<WorkflowException>(async () =>
      await handler.Handle(
        new GetBidderQuery(Guid.NewGuid()),
        TestContext.Current.CancellationToken
      ));

    Assert.Equal(404, exception.StatusCode);
    Assert.Equal("Không tìm thấy người dùng", exception.Message);
  }
}
