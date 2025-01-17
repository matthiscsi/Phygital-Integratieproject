using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PIP.BL.IManagers;
using PIP.Domain.Flow;
using UI.MVC.Models.Dto;

namespace UI.MVC.Controllers.api;

[ApiController]
[Route("/api/[controller]")]
public class SubthemesController : ControllerBase
{
    private readonly IFlowManager _flowManager;
    private readonly ISubthemeManager _subthemeManager;

    public SubthemesController(IFlowManager flowManager, ISubthemeManager subthemeManager)
    {
        _flowManager = flowManager;
        _subthemeManager = subthemeManager;
    }

    [HttpGet("flow/{id:long}")]
    [Authorize(Roles = "subplatformadministrator")]
    public IActionResult GetSubthemesOfFlow(long id)
    {
        try
        {
            var flow = _flowManager.GetFlowWithThemesAndSubthemes(id);
            bool userManager = User.Identity.Name == flow.Project.SubPlatformAdmin.UserName;
            if (!userManager)
            {
                return Unauthorized();
            }

            if (flow.Theme == null || flow.Theme.SubThemes == null)
            {
                return NotFound("Subthemes of the flow were not found.");
            }

            return Ok(flow.Theme.SubThemes);
        }
        catch (Exception ex)
        {
            return NotFound(ex.Message);
        }
    }


    [HttpPost]
    [Authorize(Roles = "subplatformadministrator")]
    [Route("post")]
    public IActionResult Create([FromBody] SubthemeDto subtheme)
    {
        var theme = _flowManager.GetFlowWithThemesAndSubthemes(subtheme.flow).Theme;
        if (ModelState.IsValid)
        {
            try
            {
                var subthemeToCreate = new Subtheme
                {
                    Title = subtheme.Title,
                    Body = subtheme.Body,
                    UrlPhoto = subtheme.UrlPhoto,
                    ParentTheme = theme
                };
                _subthemeManager.AddSubtheme(subthemeToCreate);
                return Ok(subthemeToCreate);
            }
            catch (Exception ex)
            {
                return NotFound("Internal server error: " + ex.Message);
            }
        }

        return BadRequest(ModelState);
    }


    [HttpDelete("{id:long}")]
    [Authorize(Roles = "subplatformadministrator")]
    public IActionResult Delete(long id)
    {
        var subtheme = _subthemeManager.GetSubtheme(id);
        var flow = _flowManager.GetFlowWithSubThemesExpectSelectedSubTheme(subtheme);
        if (!User.Identity!.IsAuthenticated)
        {
            return Redirect("/Identity/Account/Login");
        }
        bool userManager = User.Identity.Name == flow.Project.SubPlatformAdmin.UserName;
        if (!userManager)
        {
            return Unauthorized();
        }

        if (subtheme == null)
        {
            return NotFound();
        }

        _subthemeManager.DeleteSubtheme(id);
        return Ok();
    }
    
    [HttpPut("{id:long}")]
    [Authorize(Roles = "subplatformadministrator")]

    public IActionResult Update(long id, [FromBody] SubthemeDto subtheme)
    {
        var subthemeToUpdate = _subthemeManager.GetSubtheme(id);
        if (subthemeToUpdate == null)
        {
            return NotFound();
        }
        var flow = _flowManager.GetFlowWithSubThemesExpectSelectedSubTheme(subthemeToUpdate);
        if (!User.Identity!.IsAuthenticated)
        {
            return Redirect("/Identity/Account/Login");
        }
        bool userManager = User.Identity.Name == flow.Project.SubPlatformAdmin.UserName;
        if (!userManager)
        {
            return Unauthorized();
        }

        if (subtheme == null)
        {
            return NotFound();
        }

        subthemeToUpdate.Title = subtheme.Title;
        subthemeToUpdate.Body = subtheme.Body;
        subthemeToUpdate.UrlPhoto = subtheme.UrlPhoto;

        _subthemeManager.UpdateSubtheme(subthemeToUpdate);
        return Ok(subthemeToUpdate);
    }
}