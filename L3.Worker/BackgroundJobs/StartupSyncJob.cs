using L1.Core.Domain.Bidding.Entities;
using L1.Core.Domain.Bidding.Enums;
using L2.Application.Repositories;
using L2.Application.UseCases.Bidding.System.EndSession;
using L2.Application.UseCases.Bidding.System.StartSession;
using MediatR;
using Quartz;

namespace L3.Worker.BackgroundJobs;

[DisallowConcurrentExecution]
public class StartupSyncJob(
  IMediator mediator,
  IRepository<AuctionSession> sessionRepo
) : IJob {
  public async Task Execute(IJobExecutionContext context) {
    var ct = context.CancellationToken;

    var missedStartSessions = await sessionRepo.GetAsync(
      s =>
        s.Status == SessionStatus.Published
        && s.TimeFrame.StartTime <= DateTime.UtcNow,
      ct: ct);

    foreach (var session in missedStartSessions) {
      await mediator.Send(new StartSessionCommand(session.Id), ct);
    }

    var missedEndSessions = await sessionRepo.GetAsync(
      s => s.Status == SessionStatus.Live && s.TimeFrame.EndTime <= DateTime.UtcNow,
      ct: ct);

    foreach (var session in missedEndSessions) {
      await mediator.Send(new EndSessionCommand(session.Id), ct);
    }
  }
}
