using L0.API.Response;
using L1.Core.Exceptions;
using L2.Application.Exceptions;
using L2.Application.Ports.Logging;
using L3.Infrastructure.Exceptions;

namespace L0.API.Middlewares;

public class GlobalExceptionMiddleware(
  RequestDelegate next,
  IBusinessLogger<GlobalExceptionMiddleware> businessLogger,
  ISystemLogger<GlobalExceptionMiddleware> systemLogger
) {
  public async Task InvokeAsync(HttpContext context) {
    try {
      await next(context);
    } catch (Exception ex) {
      var shouldLogToFile = ex
        is DomainException
        or WorkflowException
        or InvalidInputException
        or InfrastructureException;

      if (shouldLogToFile) {
        businessLogger.LogError(ex, "{Message}", ex.Message);
      } else {
        systemLogger.LogError(ex, "{Message}", ex.Message);
      }


      var (statusCode, responseModel) = MapException(ex);
      context.Response.ContentType = "application/json";
      context.Response.StatusCode = statusCode;
      await context.Response.WriteAsJsonAsync(responseModel);
    }
  }

  private static (int StatusCode, object ResponseValue) MapException(Exception ex) {
    return ex switch {
      InvalidInputException iIEx => (
        422,
        AppResponse.Fail(
          iIEx.Errors,
          iIEx.Errors.FirstOrDefault() ?? "Dữ liệu không hợp lệ", 422).Value!
      ),
      DomainException dEx => (400, AppResponse.Fail(dEx.Message, 400).Value!),
      InfrastructureException iEx => (500, AppResponse.Fail(iEx.Message, 500).Value!),
      WorkflowException wfEx => (wfEx.StatusCode, AppResponse.Fail(wfEx.Message, wfEx.StatusCode).Value!),
      _ => (500, AppResponse.Fail("Lỗi hệ thống không xác định", 500).Value!)
    };
  }
}
