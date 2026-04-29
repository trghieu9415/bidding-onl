using Microsoft.AspNetCore.Mvc;

namespace L4.Presentation.Controllers;

[Route("api/external/[controller]")]
[ApiExplorerSettings(GroupName = "v3")]
public abstract class ExternalController : BaseController;
