using IP.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PIP.BL.IManagers;
using PIP.Domain.Deelplatform;
using PIP.Domain.Flow;
using UI.MVC.Models;
using UI.MVC.Models.Dto;

namespace UI.MVC.Controllers.FlowSession.Flow;

public class FlowController : Controller
{
    private readonly IFlowManager _flowManager;
    private readonly IProjectManager _projectManager;
    private readonly ICloudBucketManager _cloudBucketManager;

    public FlowController(IFlowManager flowManager, IProjectManager projectManager,
        ICloudBucketManager cloudBucketManager)
    {
        _projectManager = projectManager;
        _flowManager = flowManager;
        _cloudBucketManager = cloudBucketManager;
    }

    public IActionResult Index(long id)
    {
        Project project = _projectManager.GetProject(id);
        if (project == null)
        {
            TempData["RedirectMessage"] = "Project bestaat niet";
            return RedirectToAction("Startup","Home", new { id });
        }
        if (project.CirculaireFlow)
        {
            return RedirectToAction("CirculaireFlow", "FlowSession", new { id });
        }
            IEnumerable<PIP.Domain.Flow.Flow> flows = _flowManager.GetFlowsOfProject(id);
            ViewData["Project"] = project;
            return View(flows);
    }


    [Authorize(Roles = "subplatformadministrator")]
    public IActionResult Overzicht(long id)
    {
        var project = _projectManager.GetProject(id);
        
        if (project == null)
        {
            TempData["RedirectMessage"] = "Project bestaat niet";
            return RedirectToAction("Index","Home");
        }
        ViewData["Project"] = project;
        if (!User.Identity!.IsAuthenticated)
        {
            return Redirect("/Identity/Account/Login");
        }

        bool userManager = User.Identity.Name == project.SubPlatformAdmin.UserName;
        if (!userManager)
        {
            return Unauthorized();
        }

        IEnumerable<PIP.Domain.Flow.Flow> flows = _flowManager.GetFlowsOfProject(id);

        List<FlowDto> flowDtos = new List<FlowDto>();

        foreach (PIP.Domain.Flow.Flow flow in flows)
        {
            flowDtos.Add(new FlowDto()
            {
                Description = flow.Description,
                Id = flow.FlowId,
                ProjectName = flow.Project.Name,
                Title = flow.Title,
                UrlPhoto = flow.UrlFlowPicture
            });
        }

        ViewData["Project"] = project;
        return View(flowDtos);
    }

    public IActionResult ThankYou(long id)
    {
        PIP.Domain.Flow.Flow flow = _flowManager.GetFlowWithProject(id);
        if (flow == null)
        {
            TempData["RedirectMessage"] = "Flow bestaat niet";
            return RedirectToAction("Startup", "Home", new { id = 1 } );
        }
        var project = _projectManager.GetProject(flow.Project.ProjectId);
        ViewData["Project"] = project;
        
        _cloudBucketManager.GenerateQrCode("ThankYou", flow.FlowId);
        
        if (TempData["ValidationErrors"] != null)
        {
            ViewData["ValidationErrors"] = TempData["ValidationErrors"].ToString();
        }

        return View(flow);
    }

    [HttpGet]
    [Authorize(Roles = "subplatformadministrator")]
    public IActionResult Add()
    {
        if (!User.Identity!.IsAuthenticated)
        {
            return Redirect("/Identity/Account/Login");
        }

        if (!(User.IsInRole("subplatformadministrator")))
        {
            return Unauthorized();
        }

        return View();
    }

[HttpPost]
[Authorize(Roles = "subplatformadministrator")]
public async Task<IActionResult> Add(CreateFlowViewModel createFlowViewModel)
{
    if (!ModelState.IsValid)
    {
        return View(createFlowViewModel);
    }

    if (!User.Identity!.IsAuthenticated)
    {
        return Redirect("/Identity/Account/Login");
    }

    var project = _projectManager.GetProject(createFlowViewModel.ProjectId);
    bool userManager = User.Identity.Name == project.SubPlatformAdmin.UserName;
    if (!userManager)
    {
        return Unauthorized();
    }


    Theme theme = new Theme()
    {
        Title = createFlowViewModel.Theme.Title,
        Body = createFlowViewModel.Theme.Body,
    };

    var flow = _flowManager.AddFlow(createFlowViewModel.ProjectId, createFlowViewModel.Title,
        createFlowViewModel.Description, theme);

    string urlFlowPicture =
        await _cloudBucketManager.UploadPicture(createFlowViewModel.UploadedThemePicture, flow.FlowId, "flow");
    _cloudBucketManager.GenerateQrCode("flow", flow.FlowId);
    flow.UrlFlowPicture = urlFlowPicture;
    _flowManager.UpdateFlow(flow);


    TempData["SuccessMessage"] = "Flow is succesvol aangemaakt, maak nu de vragen aan!";

    return RedirectToAction("Details", new { flow.FlowId });
}

    [Authorize(Roles = "subplatformadministrator")]
    public IActionResult Details(long flowId)
    {
        if (!User.Identity!.IsAuthenticated)
        {
            return Redirect("/Identity/Account/Login");
        }

        var flow = _flowManager.GetFlow(flowId);
        if (flow == null)
        {
            return RedirectToAction("Index", "Home");
        }
        var project = _projectManager.GetProject(flow.Project.ProjectId);
        bool userManager = User.Identity.Name == project.SubPlatformAdmin.UserName;
        if (!userManager)
        {
            return Unauthorized();
        }

        flow = _flowManager.GetFlow(flowId);
        FlowDto flowDto = new FlowDto
        {
            Title = flow.Title,
            Description = flow.Description
        };
        ViewData["Project"] = project;
        return View(flowDto);
    }
}