using L2.Application.Ports.Security;
using MediatR;
using Microsoft.Extensions.Logging;

namespace L2.Application.UseCases.Auth.Commands.Test;

public class TestHandler(
  ILogger<TestHandler> logger,
  ICurrentUser currentUser
) : IRequestHandler<TestCommand, bool> {
  public async Task<bool> Handle(TestCommand request, CancellationToken ct) {
    if (request.Id == "error") {
      throw new Exception("Error");
    }

    Console.WriteLine($"Người dùng: {currentUser.Id}");

    logger.LogInformation(
      "[{Id}] === BẮT ĐẦU xử lý. LockKey: {Key}",
      request.Id, request.LockKey
    );
    await Task.Delay(1200, ct);
    logger.LogInformation("[{Id}] === KẾT THÚC xử lý thành công.", request.Value);

    return true;
  }
}
