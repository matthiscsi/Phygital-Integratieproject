using PIP.BL.IManagers;
using PIP.DAL.IRepositories;
using Pip.Domain.Flow;
using PIP.Domain.Flow;
using PIP.Domain.Flow.Inquiry;
using DateTime = System.DateTime;

namespace PIP.BL.Managers;

public class FlowSessionManager : IFlowSessionManager
{
    private readonly IFlowRepository _flowRepository;
    private readonly IFlowStepRepository _flowStepRepository;
    private readonly IFlowsessionRepository _flowsessionRepository;


    public FlowSessionManager(IFlowRepository flowRepository, IFlowStepRepository flowStepRepository,
        IFlowsessionRepository flowsessionRepository)
    {
        _flowRepository = flowRepository;
        _flowStepRepository = flowStepRepository;
        _flowsessionRepository = flowsessionRepository;
    }

    public FlowStep GetCurrentStep(long flowSessionId)
    {
        var flowSession = _flowRepository.ReadFlowSessionWithFlowAndSteps(flowSessionId);
        if (flowSession == null || flowSession.CurrentFlowStep == null)
        {
            return null;
        }

        return flowSession.CurrentFlowStep;
    }

    public FlowStep MoveToNextStep(long flowSessionId)
    {
        var flowSession = _flowRepository.ReadFlowSessionWithFlowAndSteps(flowSessionId);
        List<FlowStep> steps = new List<FlowStep>();
        if (flowSession == null)
        {
            return null; // Could throw an exception or handle this scenario differently based on your design
        }

        if (flowSession.Flow == null)
        {
            if (flowSession.CirculaireFlows != null)
                foreach (var flow in flowSession.CirculaireFlows.Flows)
                {
                    foreach (var flowstep in flow.FlowSteps)
                    {
                        steps.Add(flowstep);
                    }
                }

            steps = steps.Where(fs => fs.IsActive).OrderBy(fs => fs.OrderNr).ToList();
        }
        else
        {
            steps = flowSession.Flow.FlowSteps.Where(fs => fs.IsActive).OrderBy(s => s.OrderNr).ToList();
        }
        
        var currentStepIndex = steps.FindIndex(s => s.FlowStepId == flowSession.CurrentFlowStep!.FlowStepId);
        FlowStep nextStep;
        while (true)
        {
            nextStep = steps.Skip(currentStepIndex + 1)
                .FirstOrDefault(s => !flowSession.PassedSubthemes.Contains(s.SubTheme.SubthemeId));

            if (nextStep == null)
            {
                flowSession.State = State.Done;
                flowSession.SessionEndDate = DateTime.Now;
                TimeSpan timeDifference = flowSession.SessionEndDate - flowSession.SessionStartDate;
                double secondsDifference = timeDifference.TotalSeconds;
                flowSession.ElapsedTime = (int)secondsDifference;
                _flowRepository.UpdateFlowSession(flowSession);
                return null;
            }

            if (nextStep.ConditionalPointId != null)
            {
                long nextStepCondPointId = (long)nextStep.ConditionalPointId;
                nextStep = _flowStepRepository.ReadFlowStepWithConditionalPoint(nextStep.FlowStepId);
                var question = nextStep.ConditionalPoint.Question;
                Answer givenAnswer = _flowStepRepository.GetAnswerForQuestion(question.FlowStepId, flowSessionId);
                Answer criteria = _flowStepRepository.ReadCriteriaForConditionalPoint(nextStepCondPointId);
                if (givenAnswer == null || !IsConditionalPointFulfilled(givenAnswer, criteria))
                {
                    currentStepIndex++;
                    continue; 
                }
            }

            break; 
        }
        flowSession.CurrentFlowStep = nextStep;
        _flowRepository.UpdateFlowSession(flowSession);

        return flowSession.CurrentFlowStep;
    }

    public bool IsConditionalPointFulfilled(Answer givenAnswer, Answer criteria)
    {
        if (givenAnswer is ClosedAnswer closedAnswer && criteria is ClosedAnswer)
        {
            ClosedAnswer closedAnswerCriteria = criteria as ClosedAnswer;
            return (closedAnswer.SelectedAnswer == closedAnswerCriteria.SelectedAnswer);
        }

        if (givenAnswer is MultipleChoiceAnswer multipleChoiceAnswer && criteria is MultipleChoiceAnswer)
        {
            MultipleChoiceAnswer multipleChoiceAnswerCriteria = criteria as MultipleChoiceAnswer;
            var givenAnswerSet = new HashSet<long>(multipleChoiceAnswer.SelectedAnswers);
            var criteriaAnswerSet = new HashSet<long>(multipleChoiceAnswerCriteria.SelectedAnswers);

            return criteriaAnswerSet.IsSubsetOf(givenAnswerSet);
        }

        if (givenAnswer is OpenAnswer openAnswer && criteria is OpenAnswer openAnswerCriteria)
        {
            var criteriaWords = openAnswerCriteria.Answer.ToLower().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var givenAnswerText = openAnswer.Answer.ToLower();

            return criteriaWords.Any(word => givenAnswerText.Contains(word));
        }

        if (givenAnswer is RangeAnswer rangeAnswer && criteria is RangeAnswer)
        {
            RangeAnswer rangeAnswerCriteria = criteria as RangeAnswer;
            return (rangeAnswer.SelectedValue == rangeAnswerCriteria.SelectedValue);
        }
        else return false;
    }

    public FlowSession StartFlow(long flowId, int expectedUser)
    {
        var flow = _flowRepository.ReadFlowWithSteps(flowId);
        if (flow.Physical)
        {
            foreach (var fs in flow.FlowSteps)
            {
                if (fs is OpenQuestion)
                {
                    fs.IsActive = false;
                }
            }
        }
        var flowSession = new FlowSession(flow);
        flowSession.State = State.Active;
        flowSession.ExpectedUsers = expectedUser;
        
        return _flowRepository.CreateFlowSession(flowSession);
    }

    public FlowSession StartCirculaireFlow(ICollection<Flow> flows)
    {
        CirculaireFlowStrategy circulaireFlowStrategy = new CirculaireFlowStrategy() { Flows = flows };
        foreach (var flow in circulaireFlowStrategy.Flows)
        {
            if (flow.Physical)
            {
                foreach (var fs in flow.FlowSteps)
                {
                    if (fs is OpenQuestion)
                    {
                        fs.IsActive = false;
                    }
                }
            }
        }
        var flowSession = new FlowSession(circulaireFlowStrategy);
        flowSession.State = State.Active;
        return _flowRepository.CreateCirculaireFlowSession(flowSession);
    }

    public FlowSession GetFlowSession(long id)
    {
        return _flowRepository.ReadFlowSessionWithFlowAndSteps(id);
    }

    public void SkipSubtheme(long subthemeId, long flowSessionId)
    {
        _flowRepository.SkipSubtheme(subthemeId, flowSessionId);
    }

    public int GetFlowSessionCount(long flowId)
    {
        return _flowRepository.ReadFlowSessionCount(flowId);
    }

    public int GetSubthemeSkippedCount(long flowId, long subthemeId)
    {
        return _flowRepository.ReadSubthemeSkippedCountForFlow(flowId, subthemeId);
    }

    public int GetAverageTimeSpentForFlow(long flowId)
    {
        return _flowRepository.ReadAverageTimeSpentForFlow(flowId);
    }

    public int GetTotalFlowSessionCount()
    {
        return _flowsessionRepository.GetTotalFlowSessionCount();
    }
}