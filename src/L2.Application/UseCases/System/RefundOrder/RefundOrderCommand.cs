using L2.Application.Abstractions;
using MediatR;

namespace L2.Application.UseCases.System.RefundOrder;

public record RefundOrderCommand(Guid Id) : IRequest<bool>, ITransactional;
