using L2.Application.Models;

namespace L0.API.Response.ResponseModels;

public record SuccessResponse : BaseResponse {
  public string? Message { get; init; }
}

public record SuccessResponse<T> : SuccessResponse {
  public T? Data { get; init; }
  public Meta? Meta { get; init; }
}
