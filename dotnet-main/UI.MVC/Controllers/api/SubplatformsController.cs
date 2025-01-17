using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PIP.BL.IManagers;

namespace UI.MVC.Controllers.api;

[Route("api/[controller]")]
[ApiController]
public class SubplatformsController : ControllerBase
{
    private readonly ISubPlatformManager _subPlatformManager;

    public SubplatformsController(ISubPlatformManager subplatformManager)
    {
        _subPlatformManager = subplatformManager;
    }

    [HttpGet("flowSessionCount/{subplatformId:long}")]
    [Authorize(Roles = "subplatformadministrator, admin" )]
    public IActionResult GetFlowSessionCount(long subplatformId)
    {
        var subplatform = _subPlatformManager.GetSubplatform(subplatformId);
        if (subplatform == null)
        {
            return NotFound("Subplatform not found.");
        }

        if (!User.Identity!.IsAuthenticated)
        {
            return Redirect("/Identity/Account/Login");
        }


        if (!User.IsInRole("admin"))
        {
            return Unauthorized();
        }


        return Ok(_subPlatformManager.GetFlowSessionCountOfSubplatform(subplatformId));
    }

    [HttpGet("averageTime/{subplatformId:long}")]
    [Authorize(Roles = "subplatformadministrator, admin" )]
    public IActionResult GetAverageTimeSpentForFlowSessionsOfSubplatform(long subplatformId)
    {
        var subplatform = _subPlatformManager.GetSubplatform(subplatformId);
        if (subplatform == null)
        {
            return NotFound("Subplatform not found.");
        }

        if (!User.Identity!.IsAuthenticated)
        {
            return Redirect("/Identity/Account/Login");
        }

        if (!User.IsInRole("admin"))
        {
            return Unauthorized();
        }

        return Ok(_subPlatformManager.GetAverageTimeSpentForFlowSessionsOfSubplatform(subplatformId));
    }
}