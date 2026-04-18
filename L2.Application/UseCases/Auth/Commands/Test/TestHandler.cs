using MediatR;
using Microsoft.Extensions.Logging;

namespace L2.Application.UseCases.Auth.Commands.Test;

public class TestHandler(
  ILogger<TestHandler> logger
) : IRequestHandler<TestCommand, bool> {
  public async Task<bool> Handle(TestCommand request, CancellationToken ct) {
    var data = request.Data;
    if (data.Id == "error") {
      throw new Exception("Error");
    }

    Console.WriteLine($"Người dùng: {request.UserId}");

    logger.LogInformation(
      "[{Id}] === BẮT ĐẦU xử lý. LockKey: {Key}",
      data.Id, request.LockKey
    );
    await Task.Delay(1200, ct);
    logger.LogInformation("[{Id}] === KẾT THÚC xử lý thành công.", data.Value);

    return true;
  }
}
