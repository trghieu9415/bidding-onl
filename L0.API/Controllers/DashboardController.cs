using L2.Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace L0.API.Controllers;

[Authorize(Roles = nameof(UserRole.Admin))]
[Route("api/dashboard/[controller]")]
[ApiExplorerSettings(GroupName = "v2")]
public abstract class DashboardController : BaseController;
