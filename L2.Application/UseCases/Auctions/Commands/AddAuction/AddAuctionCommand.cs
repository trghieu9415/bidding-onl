using FluentValidation;
using L2.Application.Abstractions;
using MediatR;

namespace L2.Application.UseCases.Auctions.Commands.AddAuction;

public record AddAuctionCommand(
  Guid CatalogItemId,
  Guid SessionId,
  decimal StepPrice,
  decimal ReservePrice
) : IRequest<Guid>, ITransactional;

public sealed class AddAuctionCommandValidator : AbstractValidator<AddAuctionCommand> {
  public AddAuctionCommandValidator() {
    RuleFor(x => x.CatalogItemId)
      .NotEmpty()
      .WithMessage("Id sản phẩm không được để trống.");

    RuleFor(x => x.SessionId)
      .NotEmpty()
      .WithMessage("Id phiên đấu giá không được để trống.");

    RuleFor(x => x.StepPrice)
      .GreaterThan(0)
      .WithMessage("Giá bước phải lớn hơn 0.");

    RuleFor(x => x.ReservePrice)
      .GreaterThan(0)
      .WithMessage("Giá khởi điểm phải lớn hơn 0.");

    RuleFor(x => x.ReservePrice)
      .GreaterThanOrEqualTo(x => x.StepPrice)
      .WithMessage("Giá khởi điểm phải lớn hơn hoặc bằng giá bước.");
  }
}
