using FluentValidation;
using L2.Application.Abstractions;
using MediatR;

namespace L2.Application.UseCases.Items.Commands.ApproveItem;

public record ApproveItemCommand(Guid Id) : IRequest<Unit>, ITransactional;

public class ApproveItemValidator : AbstractValidator<ApproveItemCommand> {
  public ApproveItemValidator() {
    RuleFor(x => x.Id).NotEmpty().WithMessage("Id sản phẩm là bắt buộc");
  }
}
