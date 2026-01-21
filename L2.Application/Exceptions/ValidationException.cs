namespace L2.Application.Exceptions;

public class ValidationException(IEnumerable<string> errors) : System.Exception("Invalid data") {
  public IEnumerable<string> Errors { get; } = errors;
}
