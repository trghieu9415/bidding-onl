using L2.Application.Abstractions;
using MediatR;

namespace L2.Application.UseCases.System.MarkOrderAsPaid;

public record MarkOrderAsPaidCommand(Guid Id) : IRequest<bool>, ITransactional;
