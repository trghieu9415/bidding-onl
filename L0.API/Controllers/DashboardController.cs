using L2.Application.Models;
using L2.Application.Ports.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace L0.API.Controllers;

[Authorize(Roles = nameof(UserRole.Admin))]
[Route("api/dashboard/[controller]")]
[ApiExplorerSettings(GroupName = "v2")]
public abstract class DashboardController : BaseController {
  private ICurrentUser? _currentUser;

  protected ICurrentUser CurrentUser => _currentUser ??= HttpContext.RequestServices.GetRequiredService<ICurrentUser>();
}
