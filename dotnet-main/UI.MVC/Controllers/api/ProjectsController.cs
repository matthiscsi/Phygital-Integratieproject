using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PIP.BL.IManagers;
using PIP.Domain.Deelplatform;
using PIP.Domain.User;
using UI.MVC.Models.Dto;
using UI.MVC.Resources.Views.Shared;

namespace UI.MVC.Controllers.api;

[ApiController]
[Route("/api/[controller]")]
public class ProjectsController : ControllerBase
{
    private readonly IProjectManager _projectManager;
    private readonly UserManager<IdentityUser> _userManager;


    public ProjectsController(IProjectManager projectManager, UserManager<IdentityUser> userManager)
    {
        _projectManager = projectManager;
        _userManager = userManager;
    }

    [HttpGet]
    [Authorize(Roles = "subplatformadministrator")]
    public async Task<IActionResult> GetProjects()
    {
        var identityUser = await _userManager.GetUserAsync(User);
        var projects = _projectManager.GetActiveProjects(identityUser as SubPlatformAdministrator) ??
                       new List<Project>();
        return Ok(projects);
    }

    [HttpGet("Details")]
    [Authorize(Roles = "subplatformadministrator")]
    public IActionResult GetProjectsWithFlows(long id)
    {
        Project project = _projectManager.GetProject(id);
        if (!User.Identity!.IsAuthenticated)
        {
            return Redirect("/Identity/Account/Login");
        }


        bool userManager = User.Identity.Name == project.SubPlatformAdmin.UserName;
        if (!userManager)
        {
            return Unauthorized();
        }

        return Ok(project);
    }


    [HttpPut("FlowSoort")]
    [Authorize(Roles = "subplatformadministrator")]
    public IActionResult UpdateProject([FromBody] ProjectDto projectDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (!User.Identity!.IsAuthenticated)
        {
            return Redirect("/Identity/Account/Login");
        }

        var project = _projectManager.GetProject(projectDto.ProjectId);
        bool userManager = User.Identity.Name == project.SubPlatformAdmin.UserName;
        if (!userManager)
        {
            return Unauthorized();
        }

        project.CirculaireFlow = projectDto.Circulair;
        project.IsActive = projectDto.isActive;
        project.Description = projectDto.Description;
        project.Name = projectDto.Name;
        project.BackgroundColor = projectDto.BackgroundColor;
        project.Font = projectDto.Font;

        _projectManager.UpdateProject(project);
        return Ok(project);
    }


    [HttpGet("flowSessionCount/{projectId:long}")]
    [Authorize(Roles = "subplatformadministrator")]
    public IActionResult GetFlowSessionCount(long projectId)
    {
        if (!User.Identity!.IsAuthenticated)
        {
            return Redirect("/Identity/Account/Login");
        }

        var project = _projectManager.GetProject(projectId);
        bool userManager = User.Identity.Name == project.SubPlatformAdmin.UserName;
        if (!userManager)
        {
            return Unauthorized();
        }
        return Ok(_projectManager.GetFlowSessionCountOfProject(projectId));
    }
    
    [HttpGet("averageTime/{projectId:long}")]
    [Authorize(Roles = "subplatformadministrator")]
    public IActionResult GetAverageTimeSpentForFlowsOfProject(long projectId)
    {
        if (!User.Identity!.IsAuthenticated)
        {
            return Redirect("/Identity/Account/Login");
        }

        var project = _projectManager.GetProject(projectId);
        bool userManager = User.Identity.Name == project.SubPlatformAdmin.UserName;
        if (!userManager)
        {
            return Unauthorized();
        }

        return Ok(_projectManager.GetAverageTimeSpentForFlowsOfProject(projectId));
    }
}