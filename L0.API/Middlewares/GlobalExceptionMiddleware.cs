using L0.API.Response;
using L1.Core.Base.Exception;
using L2.Application.Exceptions;
using L3.Infrastructure.Exceptions;
using ValidationException = L2.Application.Exceptions.ValidationException;

namespace L0.API.Middlewares;

public class GlobalExceptionMiddleware(
  RequestDelegate next,
  ILogger<GlobalExceptionMiddleware> logger
) {
  public async Task InvokeAsync(HttpContext context) {
    try {
      await next(context);
    } catch (Exception ex) {
      var shouldLogToFile = ex
        is DomainException
        or AppException
        or ValidationException
        or InfrastructureException;

      if (shouldLogToFile) {
        logger.LogError("{Message}", ex.Message);
      } else {
        Console.WriteLine($"[UNEXPECTED ERROR] {DateTime.Now:HH:mm:ss}");
        Console.WriteLine(ex.ToString());
      }


      var (statusCode, responseModel) = MapException(ex);
      context.Response.ContentType = "application/json";
      context.Response.StatusCode = statusCode;
      await context.Response.WriteAsJsonAsync(responseModel);
    }
  }

  private static (int StatusCode, object ResponseValue) MapException(Exception ex) {
    return ex switch {
      ValidationException vEx => (
        422,
        AppResponse.Fail(
          vEx.Errors,
          vEx.Errors.FirstOrDefault() ?? "Dữ liệu không hợp lệ", 422).Value!
      ),
      DomainException dEx => (400, AppResponse.Fail(dEx.Message, 400).Value!),
      InfrastructureException iEx => (500, AppResponse.Fail(iEx.Message, 500).Value!),
      AppException appEx => (appEx.StatusCode, AppResponse.Fail(appEx.Message, appEx.StatusCode).Value!),
      _ => (500, AppResponse.Fail("Lỗi hệ thống không xác định", 500).Value!)
    };
  }
}
