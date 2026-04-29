using FluentValidation;
using L2.Application.Abstractions;
using MediatR;

namespace L2.Application.UseCases.Items.Commands.RejectItem;

public record RejectItemCommand(Guid Id, RejectItemRequest Data) : IRequest<bool>, ITransactional;

public record RejectItemRequest(string Reason);

public class RejectItemValidator : AbstractValidator<RejectItemCommand> {
  public RejectItemValidator() {
    RuleFor(x => x.Id).NotEmpty().WithMessage("Id sản phẩm là bắt buộc");
  }
}
