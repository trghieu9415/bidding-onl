using FluentValidation;
using L2.Application.Abstractions;
using MediatR;

namespace L2.Application.UseCases.Sessions.Commands.UpdateSession;

public record UpdateSessionCommand(
  Guid Id,
  UpdateSessionRequest Data
) : IRequest<bool>, ITransactional;

public record UpdateSessionRequest(string Title, DateTime StartTime, DateTime EndTime);

public sealed class UpdateSessionValidator : AbstractValidator<UpdateSessionRequest> {
  public UpdateSessionValidator() {
    RuleFor(x => x.Title)
      .NotEmpty()
      .WithMessage("Tiêu đề phiên đấu giá không được để trống.")
      .MaximumLength(200)
      .WithMessage("Tiêu đề phiên đấu giá không được vượt quá 200 ký tự.");

    RuleFor(x => x.EndTime)
      .GreaterThan(x => x.StartTime)
      .WithMessage("Thời gian kết thúc phải lớn hơn thời gian bắt đầu.");
  }
}
