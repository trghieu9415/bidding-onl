using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace L4.Presentation.Hubs;

[Authorize]
public class UserHub : Hub;
