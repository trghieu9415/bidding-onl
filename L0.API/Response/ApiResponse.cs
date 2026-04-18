using L2.Application.Models;
using Microsoft.AspNetCore.Mvc;

namespace L0.API.Response;

public static class ApiResponse {
  private const string DefaultSuccess = "Thao tác thành công";
  private const string DefaultError = "Đã có lỗi xảy ra!!!";

  private static ObjectResult BuildSuccess<T>(int status, string? message, T data, Meta? meta = null) {
    var response = new ResponseData<T> {
      Status = status,
      Message = message ?? DefaultSuccess,
      Data = data, Meta = meta
    };
    return new ObjectResult(response) { StatusCode = status };
  }

  private static ObjectResult BuildFail(int status, string? error, object? data = null) {
    object response = data == null
      ? new { Status = status, Error = error ?? DefaultError }
      : new { Status = status, Error = error ?? DefaultError, Data = data };

    return new ObjectResult(response) { StatusCode = status };
  }

  public static ObjectResult Success(string? message = null) {
    return BuildSuccess<object>(200, message, null!);
  }

  public static ObjectResult Success(string? message, int status) {
    return BuildSuccess<object>(status, message, null!);
  }

  public static ObjectResult Success<T>(T data, string? message = null, int status = 200) {
    return BuildSuccess(status, message, data);
  }

  public static ObjectResult Success<T>(T data, Meta meta, string? message = null, int status = 200) {
    return BuildSuccess(status, message, data, meta);
  }

  public static ObjectResult Success(Guid id, string? message = null, int status = 200) {
    return BuildSuccess(status, message, new IdData { Id = id });
  }

  public static ObjectResult Fail(string? error = null, int status = 400) {
    return BuildFail(status, error);
  }

  public static ObjectResult Fail<T>(T data, string? error = null, int status = 400) {
    return BuildFail(status, error, data);
  }
}
