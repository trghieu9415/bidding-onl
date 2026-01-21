namespace L0.API.Response.ResponseModels;

public abstract record BaseResponse {
  public int Status { get; init; }
}
