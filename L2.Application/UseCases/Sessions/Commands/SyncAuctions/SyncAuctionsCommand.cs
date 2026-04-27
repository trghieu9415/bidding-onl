using FluentValidation;
using L2.Application.Abstractions;
using MediatR;

namespace L2.Application.UseCases.Sessions.Commands.SyncAuctions;

public record SyncAuctionsCommand(Guid Id, List<Guid> AuctionIds) : IRequest<bool>, ITransactional;

public sealed class SyncAuctionsValidator : AbstractValidator<SyncAuctionsCommand> {
  public SyncAuctionsValidator() {
    RuleFor(x => x.Id)
      .NotEmpty()
      .WithMessage("Id phiên đấu giá không được để trống.");

    RuleFor(x => x.AuctionIds)
      .NotNull()
      .WithMessage("Danh sách phiên đấu giá không được để trống.");

    RuleForEach(x => x.AuctionIds)
      .NotEmpty()
      .WithMessage("Id phiên đấu giá không hợp lệ.");
  }
}
