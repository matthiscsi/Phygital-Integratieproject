using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PIP.BL.IManagers;
using UI.MVC.Models.Dto;

namespace UI.MVC.Controllers;

public class SubthemeController : Controller
{
    private readonly ISubthemeManager _subthemeManager;
    private readonly ICloudBucketManager _cloudBucketManager;
    private readonly IProjectManager _projectManager;

    public SubthemeController(ISubthemeManager subthemeManager, ICloudBucketManager cloudBucketManager,
        IProjectManager projectManager)
    {
        _subthemeManager = subthemeManager;
        _cloudBucketManager = cloudBucketManager;
        _projectManager = projectManager;
    }

    [Authorize(Roles = "subplatformadministrator")]
    public IActionResult Edit(long id)
    {
        var subtheme = _subthemeManager.GetSubtheme(id);
        if (subtheme == null)
        {
            return NotFound();
        }

        if (!User.Identity!.IsAuthenticated)
        {
            return Redirect("/Identity/Account/Login");
        }

        var project = _projectManager.ReadProjectFromSubtheme(id);
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (project.SubPlatformAdmin.Id != userId)
        {
            return Redirect("/Identity/Account/AccessDenied");
        }


        return View(subtheme);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(SubthemeDto subthemeDto)
    {
        var subtheme = _subthemeManager.GetSubtheme(subthemeDto.Id);

        if (subtheme == null)
        {
            return NotFound();
        }

        var project = _projectManager.ReadProjectFromSubtheme(subthemeDto.Id);
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (project.SubPlatformAdmin.Id != userId)
        {
            return Redirect("/Identity/Account/AccessDenied");
        }

        if (ModelState.IsValid)
        {
            subtheme.Title = subthemeDto.Title;
            subtheme.Body = subthemeDto.Body;

            if (subthemeDto.Photo != null && subthemeDto.Photo.Length > 0)
            {
                var urlPhoto = await _cloudBucketManager.UploadPicture(subthemeDto.Photo, subthemeDto.Id, "subtheme");
                subtheme.UrlPhoto = urlPhoto;
            }

            _subthemeManager.UpdateSubtheme(subtheme);
            TempData["Message"] = "Subtheme is bijgewerkt";
            return View(subtheme);
        }
        
        return View(subtheme);
    }
}