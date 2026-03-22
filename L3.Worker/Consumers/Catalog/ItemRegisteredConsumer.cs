using L1.Core.Domain.Catalog.Events;
using L2.Application.Models;
using L2.Application.Ports.Realtime;
using L3.Infrastructure.Persistence;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace L3.Worker.Consumers.Catalog;

public class ItemRegisteredConsumer(
  AppDbContext dbContext,
  IUserNotifier userNotifier
) : IConsumer<ItemRegisteredEvent> {
  public async Task Consume(ConsumeContext<ItemRegisteredEvent> context) {
    var msg = context.Message;

    var adminIds = await dbContext.Users
      .AsNoTracking()
      .Where(u => u.Role == UserRole.Admin)
      .Select(u => u.Id)
      .ToListAsync();

    foreach (var adminId in adminIds) {
      await userNotifier.SendToUser(
        adminId,
        "ItemRegistered",
        new {
          msg.ItemId,
          msg.OwnerId
        },
        context.CancellationToken);
    }
  }
}
