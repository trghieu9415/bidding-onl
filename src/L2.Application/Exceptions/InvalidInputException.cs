namespace L2.Application.Exceptions;

public class InvalidInputException(IEnumerable<string> errors) : Exception("Đầu vào không hợp lệ") {
  public IEnumerable<string> Errors { get; } = errors;
}
