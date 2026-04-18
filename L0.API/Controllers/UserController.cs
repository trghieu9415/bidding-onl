using L2.Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace L0.API.Controllers;

[Authorize(Roles = nameof(UserRole.Bidder))]
[Route("api/user/[controller]")]
[ApiExplorerSettings(GroupName = "v1")]
[EnableRateLimiting("PublicApiPolicy")]
public abstract class UserController : BaseController;
