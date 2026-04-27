using FluentValidation;
using L2.Application.Abstractions;
using MediatR;

namespace L2.Application.UseCases.Bids.Commands.PlaceBid;

public record PlaceBidCommand(
  Guid AuctionId,
  Guid UserId,
  string UserFullName,
  PlaceBidRequest Data
) : IRequest<Guid>, ITransactional, ILockable {
  public string LockKey => $"locks:auction:{AuctionId}";
  public TimeSpan WaitTime => TimeSpan.FromSeconds(5);
}

public sealed class PlaceBidValidator : AbstractValidator<PlaceBidCommand> {
  public PlaceBidValidator() {
    RuleFor(x => x.AuctionId)
      .NotEmpty()
      .WithMessage("Id phiên đấu giá không được để trống.");

    RuleFor(x => x.UserId)
      .NotEmpty()
      .WithMessage("Id người dùng không được để trống.");

    RuleFor(x => x.UserFullName)
      .NotEmpty()
      .WithMessage("Tên người đấu giá không được để trống.")
      .MaximumLength(200)
      .WithMessage("Tên người đấu giá không được vượt quá 200 ký tự.");

    RuleFor(x => x.Data)
      .NotNull()
      .WithMessage("Thông tin đấu giá không được để trống.");
  }
}

public record PlaceBidRequest(decimal Amount);
