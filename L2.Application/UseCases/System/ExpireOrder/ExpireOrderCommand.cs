using L2.Application.Abstractions;
using MediatR;

namespace L2.Application.UseCases.System.ExpireOrder;

public record ExpireOrderCommand(Guid Id) : IRequest<bool>, ITransactional;
