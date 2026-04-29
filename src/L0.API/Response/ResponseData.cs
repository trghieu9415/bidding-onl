using L2.Application.Models;

namespace L0.API.Response;

public record ResponseData<T> {
  public int Status { get; init; }
  public string? Message { get; init; }
  public string? Error { get; init; }
  public T? Data { get; init; }
  public Meta? Meta { get; init; }
}

public record IdData {
  public Guid Id { get; init; }
}
