using System.Diagnostics;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using PIP.BL.IManagers;
using UI.MVC.Models;

namespace UI.MVC.Controllers;

public class HomeController : Controller
{
    private readonly IProjectManager _projectManager;
    private readonly IFlowSessionManager _flowSessionManager;


    public HomeController(IProjectManager projectManager, IFlowSessionManager iFlowSessionManager)
    {
        _flowSessionManager = iFlowSessionManager;
        _projectManager = projectManager;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Startup(long id)
    {
        var project = _projectManager.GetProjectWithSubplatform(id);
        if (project == null)
        {
            return RedirectToAction("Startup", new { id = 1 });
        }

        ViewData["Project"] = project;
        return View(project);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Contact()
    {
        return View();
    }

    public IActionResult WaitScreen(long flowsessionId)
    {
        var flowSession = _flowSessionManager.GetFlowSession(flowsessionId);
        if (flowSession == null)
        {
            return RedirectToAction("Startup", new { id = 1 });
        }

        ViewData["Project"] = flowSession.CirculaireFlows!.Flows.FirstOrDefault()!.Project;

        return View(flowSession);
    }


    [HttpPost]
    public IActionResult Contact(ContactViewModel contactVm)
    {
        if (ModelState.IsValid)
            return RedirectToAction("Index");

        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [HttpPost]
    public IActionResult CultureChange(string culture, string returnUrl)
    {
        Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
            new CookieOptions { Expires = DateTimeOffset.Now.AddDays(1) }
        );

        return LocalRedirect(returnUrl);
    }
}