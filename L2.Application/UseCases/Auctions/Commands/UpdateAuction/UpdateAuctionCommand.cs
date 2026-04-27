using FluentValidation;
using L2.Application.Abstractions;
using MediatR;

namespace L2.Application.UseCases.Auctions.Commands.UpdateAuction;

public record UpdateAuctionCommand(
  Guid Id,
  UpdateAuctionRequest Data
) : IRequest<bool>, ITransactional;

public sealed class UpdateAuctionValidator : AbstractValidator<UpdateAuctionRequest> {
  public UpdateAuctionValidator() {
    RuleFor(x => x.StepPrice).GreaterThan(0).WithMessage("Giá bước phải lớn hơn 0.");
    RuleFor(x => x.ReservePrice).GreaterThan(0).WithMessage("Giá khởi điểm phải lớn hơn 0.");
    RuleFor(x => x.ReservePrice).GreaterThanOrEqualTo(x => x.StepPrice)
      .WithMessage("Giá khởi điểm phải lớn hơn hoặc bằng giá bước.");
  }
}

public record UpdateAuctionRequest(
  decimal StepPrice,
  decimal ReservePrice
);
