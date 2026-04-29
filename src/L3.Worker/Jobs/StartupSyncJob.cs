using Hangfire;
using L1.Core.Domain.Bidding.Entities;
using L1.Core.Domain.Bidding.Enums;
using L2.Application.Repositories;
using L2.Application.UseCases.System.EndSession;
using L2.Application.UseCases.System.StartSession;
using MediatR;

namespace L3.Worker.Jobs;

[DisableConcurrentExecution(300)]
public class StartupSyncJob(
  IMediator mediator,
  IRepository<AuctionSession> sessionRepo
) {
  public async Task Execute(CancellationToken ct) {
    var missedStartSessions = await sessionRepo.GetAsync(
      s => s.Status == SessionStatus.Published && s.TimeFrame.StartTime <= DateTime.UtcNow,
      ct: ct
    );

    foreach (var session in missedStartSessions) {
      await mediator.Send(new StartSessionCommand(session.Id), ct);
    }

    var missedEndSessions = await sessionRepo.GetAsync(
      s => s.Status == SessionStatus.Live && s.TimeFrame.EndTime <= DateTime.UtcNow,
      ct: ct
    );

    foreach (var session in missedEndSessions) {
      await mediator.Send(new EndSessionCommand(session.Id), ct);
    }
  }
}
