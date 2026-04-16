using L0.API.Response;
using L1.Core.Exceptions;
using L2.Application.Exceptions;
using L2.Application.Ports.Logging;
using L3.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

namespace L0.API.ExceptionHandler;

public class GlobalExceptionHandler(
  IBusinessLogger<GlobalExceptionHandler> businessLogger,
  ISystemLogger<GlobalExceptionHandler> systemLogger
) : IExceptionHandler {
  public async ValueTask<bool> TryHandleAsync(
    HttpContext context,
    Exception exception,
    CancellationToken ct
  ) {
    var shouldLogToFile = exception
      is DomainException
      or WorkflowException
      or InvalidInputException
      or InfrastructureException;

    if (shouldLogToFile) {
      businessLogger.LogError(exception, "{Message}", exception.Message);
    } else {
      systemLogger.LogError(exception, "{Message}", exception.Message);
    }

    var (statusCode, responseModel) = MapException(exception);

    context.Response.ContentType = "application/json";
    context.Response.StatusCode = statusCode;

    await context.Response.WriteAsJsonAsync(responseModel, ct);
    return true;
  }

  private static (int StatusCode, object ResponseValue) MapException(Exception ex) {
    return ex switch {
      InvalidInputException iIEx => (
        422,
        ApiResponse.Fail(
          iIEx.Errors,
          iIEx.Errors.FirstOrDefault() ?? "Dữ liệu không hợp lệ", 422).Value!
      ),
      DomainException dEx => (400, ApiResponse.Fail(dEx.Message, 400).Value!),
      InfrastructureException iEx => (500, ApiResponse.Fail(iEx.Message, 500).Value!),
      WorkflowException wfEx => (wfEx.StatusCode, ApiResponse.Fail(wfEx.Message, wfEx.StatusCode).Value!),
      _ => (500, ApiResponse.Fail("Lỗi hệ thống không xác định", 500).Value!)
    };
  }
}
