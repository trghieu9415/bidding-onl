using FluentValidation;
using L1.Core.Domain.Catalog.Enums;
using L2.Application.Abstractions;
using MediatR;

namespace L2.Application.UseCases.Items.Commands.RegisterItem;

public record RegisterItemCommand(
  Guid UserId,
  RegisterItemRequest Data
) : IRequest<Guid>, ITransactional;

public record RegisterItemRequest(
  string Name,
  string Description,
  decimal StartingPrice,
  ItemCondition Condition,
  List<Guid> CategoryIds,
  string? MainImageUrl,
  List<string> SubImageUrls
);

public class RegisterItemValidator : AbstractValidator<RegisterItemCommand> {}
