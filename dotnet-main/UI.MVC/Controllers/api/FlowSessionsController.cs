using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PIP.BL.IManagers;
using PIP.Domain.Companion;
using PIP.Domain.Flow;
using UI.MVC.Models.Dto;

namespace UI.MVC.Controllers.api;

[ApiController]
[Route("/api/[controller]")]
public class FlowSessionsController : ControllerBase
{
    private readonly IFlowSessionManager _flowSessionManager;
    private readonly IFlowManager _flowManager;
    private readonly IProjectManager _projectManager;

    public FlowSessionsController(IFlowSessionManager flowSessionManager, IFlowManager flowManager,
        IProjectManager projectManager)
    {
        _flowSessionManager = flowSessionManager;
        _flowManager = flowManager;
        _projectManager = projectManager;
    }

    [HttpGet("subthemeSkippedCount/{flowId:long}")]
    [Authorize(Roles = "subplatformadministrator")]
    public IActionResult GetSubthemeSkippedCount(long flowId)
    {
        if (!User.Identity!.IsAuthenticated)
        {
            return Redirect("/Identity/Account/Login");
        }

        var project = _projectManager.ReadProjectFromFlow(flowId);
        bool userManager = User.Identity.Name == project.SubPlatformAdmin.UserName;
        if (!userManager)
        {
            return Unauthorized();
        }

        List<SkippedSubthemeAnalysisDto> skippedSubthemeAnalysisDtos = new List<SkippedSubthemeAnalysisDto>();
        var subthemes = _flowManager.GetSubthemesOfFlow(flowId);
        foreach (Subtheme subtheme in subthemes)
        {
            skippedSubthemeAnalysisDtos.Add(new SkippedSubthemeAnalysisDto()
            {
                SubthemeDto = new SubthemeDto
                {
                    Title = subtheme.Title
                },
                Quantity = _flowSessionManager.GetSubthemeSkippedCount(flowId, subtheme.SubthemeId)
            });
        }

        return Ok(skippedSubthemeAnalysisDtos);
    }

    [HttpGet("flowSessionCount/{flowId:long}")]
    [Authorize(Roles = "subplatformadministrator")]
    public IActionResult GetFlowSessionCount(long flowId)
    {
        if (!User.Identity!.IsAuthenticated)
        {
            return Redirect("/Identity/Account/Login");
        }

        var project = _projectManager.ReadProjectFromFlow(flowId);
        bool userManager = User.Identity.Name == project.SubPlatformAdmin.UserName;
        if (!userManager)
        {
            return Unauthorized();
        }

        int flowSessionCount = _flowSessionManager.GetFlowSessionCount(flowId);

        return Ok(flowSessionCount);
    }

    [HttpGet("averageTimeSpent/{flowId:long}")]
    [Authorize(Roles = "subplatformadministrator")]
    public IActionResult GetAverageTimeSpentForFlow(long flowId)
    {
        if (!User.Identity!.IsAuthenticated)
        {
            return Redirect("/Identity/Account/Login");
        }

        var project = _projectManager.ReadProjectFromFlow(flowId);
        bool userManager = User.Identity.Name == project.SubPlatformAdmin.UserName;
        if (!userManager)
        {
            return Unauthorized();
        }

        int? averageTimeSpent = _flowSessionManager.GetAverageTimeSpentForFlow(flowId);
        return Ok(averageTimeSpent);
    }

    [HttpGet("flowSessionCount")]
    [Authorize(Roles = "subplatformadministrator")]
    public IActionResult GetFlowSessionCount()
    {
        int flowSessionCount = _flowSessionManager.GetTotalFlowSessionCount();

        return Ok(flowSessionCount);
    }

    [HttpPost("startFlowsession/{id:long}")]
    public IActionResult StartFlow(long id, [FromQuery] int expectedUser)
    {
        var flowSession = _flowSessionManager.StartFlow(id, expectedUser);
        return Ok(flowSession);
    }

    [HttpPost("notitie")]
    [Authorize(Roles = "companion")]
    public IActionResult AddNote([FromBody] NoteDto noteDto)
    {
        var flowSession = _flowSessionManager.GetFlowSession(noteDto.flowsessionId);
        var currentflowstep = flowSession.CurrentFlowStep;
        if (currentflowstep != null) noteDto.flowstepId = currentflowstep.FlowStepId;
        if (!User.Identity!.IsAuthenticated)
        {
            return Redirect("/Identity/Account/Login");
        }

        Note note = _flowManager.AddNote(noteDto.Title, noteDto.Description, noteDto.flowsessionId, noteDto.flowstepId);
        return Ok(note);
    }
}