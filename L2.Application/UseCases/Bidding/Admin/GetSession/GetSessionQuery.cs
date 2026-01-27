using L2.Application.Abstractions;

namespace L2.Application.UseCases.Bidding.Admin.GetSession;

public record GetSessionQuery(Guid Id) : IQuery<GetSessionResult>;
