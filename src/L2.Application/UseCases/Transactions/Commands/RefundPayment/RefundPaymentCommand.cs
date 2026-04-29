using L2.Application.Abstractions;
using MediatR;

namespace L2.Application.UseCases.Transactions.Commands.RefundPayment;

public record RefundPaymentCommand(Guid Id, Guid UserId) : IRequest<bool>, ITransactional;
