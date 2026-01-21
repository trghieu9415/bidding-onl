namespace L0.API.Response.ResponseModels;

public record ErrorResponse : BaseResponse {
  public string Error { get; init; } = "Có lỗi xảy ra!!";
}

public record ErrorResponse<T> : ErrorResponse {
  public T? Data { get; init; }
}
