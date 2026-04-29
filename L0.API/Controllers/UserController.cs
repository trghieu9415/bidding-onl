using L2.Application.Models;
using L2.Application.Ports.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace L0.API.Controllers;

[Authorize(Roles = nameof(UserRole.Bidder))]
[Route("api/user/[controller]")]
[ApiExplorerSettings(GroupName = "v1")]
public abstract class UserController : BaseController {
  private ICurrentUser? _currentUser;

  protected ICurrentUser CurrentUser => _currentUser ??= HttpContext.RequestServices.GetRequiredService<ICurrentUser>();
}
