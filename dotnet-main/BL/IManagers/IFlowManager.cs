using PIP.Domain.Companion;
using PIP.Domain.Flow;

namespace PIP.BL.IManagers;

public interface IFlowManager
{
    
    IEnumerable<Flow> GetFlowsOfProject(long id);
    Flow GetFlow(long id);
    Flow GetFlowWithSteps(long id);
    Flow GetFlowWithProject(long id);
    Flow GetFlowWithThemesAndIdeeas(long flowId); 
    Flow AddFlow(long projectId, string title, string description,Theme theme);
    Flow GetFlowWithThemesAndSubthemes(long flowId);
    Flow GetFlowWithSubThemesExpectSelectedSubTheme(Subtheme subtheme);
    IEnumerable<Subtheme> GetSubthemesOfFlow(long flowId);
    void UpdateFlow(Flow flow);
    void UpdateFlowSessionCirculaireFlow(long id);
    Note AddNote(string title, string description, long flowsessionId, long flowstepId);
    Flow GetFlowFromFlowstep(long flowstepId);
}