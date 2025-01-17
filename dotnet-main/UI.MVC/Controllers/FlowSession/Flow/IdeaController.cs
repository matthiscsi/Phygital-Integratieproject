using Microsoft.AspNetCore.Mvc;
using PIP.BL;
using PIP.BL.IManagers;
using PIP.Domain.WebApplication;
using UI.MVC.Models;

namespace UI.MVC.Controllers.FlowSession.Flow;

public class IdeaController : Controller
{
    private readonly IIdeaManager _ideaManager;

    private readonly IProfanityFilter _profanityFilter;
    private readonly IFlowManager _flowManager;
    private readonly IProjectManager _projectManager;
    private readonly ICloudBucketManager _cloudBucketManager;

    public IdeaController(IIdeaManager ideaManager, IProfanityFilter profanityFilter, IFlowManager flowManager,
        IProjectManager projectManager, ICloudBucketManager cloudBucketManager)
    {
        _ideaManager = ideaManager;
        _profanityFilter = profanityFilter;
        _flowManager = flowManager;
        _projectManager = projectManager;
        _cloudBucketManager = cloudBucketManager;
    }

    public IActionResult Index(long flowId)
    {
        var flow = _flowManager.GetFlowWithThemesAndIdeeas(flowId);
        if (flow == null)
            return RedirectToAction("Startup", "Home");
        var project = _projectManager.GetProject(flow.Project.ProjectId);
        ViewData["Project"] = project;
        return View(flow);
    }

    public IActionResult Create(long flowId)
    {
        PIP.Domain.Flow.Flow flow = _flowManager.GetFlowWithProject(flowId);
        if (flow == null)
            return RedirectToAction("Index", "Home");
        var project = _projectManager.GetProject(flow.Project.ProjectId);
        ViewData["Project"] = project;
        var viewModel = new CreateIdeaViewModel { FlowId = flowId };
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateIdeaViewModel viewModel)
    {
        PIP.Domain.Flow.Flow flow = _flowManager.GetFlowWithProject(viewModel.FlowId);
        var project = _projectManager.GetProject(flow.Project.ProjectId);
        
        if (!ModelState.IsValid)
        {
            ViewData["Project"] = project;
            return View(viewModel);
        }

        if (_profanityFilter.ContainsProfanity(viewModel.Title) || _profanityFilter.ContainsProfanity(viewModel.Description))
        {
            ModelState.AddModelError(string.Empty,
                "The title or description contains prohibited words. Please remove them.");
            ViewData["Project"] = project;
            return View(viewModel);
        }
        var idea = new Idea
        {   
            Title = viewModel.Title,
            Description = viewModel.Description
        };
        if (viewModel.Photo!=null)
        {
                    string urlIdeaPicture = await _cloudBucketManager.UploadPicture(viewModel.Photo, idea.IdeaId, "ideaPicture");
                    
                    idea.UrlPhoto = urlIdeaPicture;
        }

        
        _flowManager.UpdateFlow(flow);
        

        _ideaManager.AddIdeaToFlow(idea, viewModel.FlowId);

        return RedirectToAction("Index", "Idea", new { flowId = viewModel.FlowId });
    }
}