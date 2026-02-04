using MediatR;
using Microsoft.Extensions.Logging;

namespace L2.Application.UseCases.Bidding.Bidder.Test;

public class TestHandler(ILogger<TestHandler> logger) : IRequestHandler<TestCommand, Unit> {
  public async Task<Unit> Handle(TestCommand request, CancellationToken ct) {
    logger.LogInformation(
      "[{Id}] === BẮT ĐẦU xử lý. LockKey: {Key}",
      request.Id, request.LockKey
    );
    await Task.Delay(3000, ct);
    logger.LogInformation("[{Id}] === KẾT THÚC xử lý thành công.", request.Value);

    return Unit.Value;
  }
}
