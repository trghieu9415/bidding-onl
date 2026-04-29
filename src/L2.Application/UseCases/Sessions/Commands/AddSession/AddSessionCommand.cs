using FluentValidation;
using L2.Application.Abstractions;
using MediatR;

namespace L2.Application.UseCases.Sessions.Commands.AddSession;

public record AddSessionCommand(
  string Title,
  DateTime StartTime,
  DateTime EndTime,
  List<Guid> AuctionIds
) : IRequest<Guid>, ITransactional;

public sealed class AddSessionValidator : AbstractValidator<AddSessionCommand> {
  public AddSessionValidator() {
    RuleFor(x => x.Title)
      .NotEmpty()
      .WithMessage("Tiêu đề phiên đấu giá không được để trống.")
      .MaximumLength(200)
      .WithMessage("Tiêu đề phiên đấu giá không được vượt quá 200 ký tự.");

    RuleFor(x => x.StartTime)
      .GreaterThan(DateTime.UtcNow)
      .WithMessage("Thời gian bắt đầu phải lớn hơn thời gian hiện tại.");

    RuleFor(x => x.EndTime)
      .GreaterThan(x => x.StartTime)
      .WithMessage("Thời gian kết thúc phải lớn hơn thời gian bắt đầu.");

    RuleFor(x => x.AuctionIds)
      .NotNull()
      .WithMessage("Danh sách phiên đấu giá không được để trống.");

    RuleForEach(x => x.AuctionIds)
      .NotEmpty()
      .WithMessage("Id phiên đấu giá không hợp lệ.");
  }
}
