using Microsoft.AspNetCore.Mvc;

namespace L0.API.Controllers;

[ApiController]
[Route("api/dashboard/[controller]")]
[ApiExplorerSettings(GroupName = "v1")]
public abstract class DashboardController : ControllerBase;