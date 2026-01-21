namespace L2.Application.Exceptions;

public class AppException(string message, int statusCode = 400) : System.Exception(message) {
  public int StatusCode { get; } = statusCode;
}
