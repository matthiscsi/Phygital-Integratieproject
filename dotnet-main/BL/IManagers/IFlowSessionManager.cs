using Pip.Domain.Flow;
using PIP.Domain.Flow;

namespace PIP.BL.IManagers;

public interface IFlowSessionManager
{
    //FLOWSESSIONMANAGER GETNEXT STEP
    public FlowStep GetCurrentStep(long flowSessionId);
    public FlowStep MoveToNextStep(long flowSessionId);
    public FlowSession StartFlow(long flowId, int expectedUser);
    public FlowSession StartCirculaireFlow(ICollection<Flow> flowSteps);

    
    public FlowSession GetFlowSession(long id);
    void SkipSubtheme(long subthemeId, long flowSessionId);
    int GetFlowSessionCount(long flowId);
    int GetSubthemeSkippedCount(long flowId, long subthemeId);
    int GetAverageTimeSpentForFlow(long flowId);
    int GetTotalFlowSessionCount();
}