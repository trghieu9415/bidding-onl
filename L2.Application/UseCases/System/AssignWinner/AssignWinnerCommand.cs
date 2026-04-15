using FluentValidation;
using L2.Application.Abstractions;
using MediatR;

namespace L2.Application.UseCases.System.AssignWinner;

public record AssignWinnerCommand(Guid CatalogItemId, bool IsSold) : IRequest<Unit>, ITransactional;

public class AssignWinnerValidator : AbstractValidator<AssignWinnerCommand> {
  public AssignWinnerValidator() {
    RuleFor(x => x.CatalogItemId)
      .NotEmpty().WithMessage("Id sản phẩm không được để trống");
  }
}
