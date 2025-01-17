using Microsoft.AspNetCore.Mvc;
using PIP.BL.IManagers;

namespace UI.MVC.Controllers.FlowSession;

public class FlowSessionController : Controller
{
    private readonly IFlowManager _flowManager;

    private readonly IFlowSessionManager _flowSessionManager;


    public FlowSessionController(IFlowManager flowManager, IFlowSessionManager flowSessionManager)
    {
        _flowManager = flowManager;
        _flowSessionManager = flowSessionManager;
    }

    public IActionResult Index(long id)
    {
        var flowSession = _flowSessionManager.GetFlowSession(id);
        return RedirectToAction("Index", "FlowStep", new { id = flowSession.FlowSessionId });
    }

    public IActionResult CirculaireFlow(long id)
    {
        ICollection<PIP.Domain.Flow.Flow> flows = _flowManager.GetFlowsOfProject(id).ToList();
        var flowSession = _flowSessionManager.StartCirculaireFlow(flows);

        return RedirectToAction("Index", "FlowStep", new { id = flowSession.FlowSessionId });
    }
}