using PIP.Domain.Companion;
using Pip.Domain.Flow;
using PIP.Domain.Flow;

namespace PIP.DAL.IRepositories;

public interface IFlowRepository
{
    public IEnumerable<Flow> ReadAllFlows();
    public Flow ReadFlow(long flowId);
    public IEnumerable<Flow> ReadFlowsOfProject(long id);
    public Flow ReadFlowWithSteps(long flowId);
    Flow ReadFlowWithProject(long id);
    FlowSession CreateFlowSession(FlowSession flowSession);
    FlowSession CreateCirculaireFlowSession(FlowSession flowSession);
    FlowSession ReadFlowSessionWithFlowAndSteps(long flowSessionId);
    void UpdateFlowSession(FlowSession flowSession);
    void SkipSubtheme(long subthemeId, long flowSessionId);
    Flow CreateFlow(Flow flow);
    Flow ReadFlowWithThemesAndIdeas(long flowId);
    Flow ReadFlowWithThemesAndSubthemes(long flowId);
    Flow ReadFlowWithSubThemesExpectSelectedSubTheme(Subtheme subtheme);
    int ReadFlowSessionCount(long flowId);
    int ReadSubthemeSkippedCountForFlow(long flowId, long subthemeId);
    IEnumerable<Subtheme> ReadSubthemesOfFlow(long flowId);
    void UpdateFlow(Flow flow);
    int ReadAverageTimeSpentForFlow(long flowId);

    void CreateNote(Note note);
    IEnumerable<Note> ReadNotesOfFlowSession(long id);
    Note UpdateNote(string title, string description);
    Flow ReadFlowFromFlowstep(long flowstepId);
    
 
}