using L2.Application.Abstractions;
using MediatR;

namespace L2.Application.UseCases.System.RefundPayment;

public record RefundPaymentCommand(Guid Id) : IRequest<bool>, ITransactional;
