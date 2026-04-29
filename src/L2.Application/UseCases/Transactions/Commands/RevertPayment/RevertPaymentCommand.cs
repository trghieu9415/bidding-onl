using L2.Application.Abstractions;
using MediatR;

namespace L2.Application.UseCases.Transactions.Commands.RevertPayment;

public record RevertPaymentCommand(Guid Id) : IRequest<bool>, ITransactional;
