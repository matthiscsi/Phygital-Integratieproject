using PIP.DAL.IRepositories;
using PIP.BL.IManagers;
using PIP.Domain.Flow;
using PIP.Domain.Flow.Inquiry;

namespace PIP.BL.Managers;

public class FlowStepManager : IFlowStepManager
{
    private readonly IFlowStepRepository _repo;
    private readonly IFlowManager _manager;
    public FlowStepManager(IFlowStepRepository repository, IFlowManager flowManager)
    {
        _repo = repository;
        _manager = flowManager;
    }
    
    public Info AddInfo( Info info)
    {
        return _repo.CreateInfo(info);
    }

    public Info GetInfo(long id)
    {
        return _repo.ReadInfo(id);
    }

    public IEnumerable<Info> GetAllInfo()
    {
        return _repo.ReadAllInfo();
    }
    
    public Question AddQuestion(Question question)
    {
        return _repo.CreateQuestion(question);
    }
    
    public Question GetQuestion(long id)
    {
        return _repo.ReadQuestion(id);
    }
    
    public IEnumerable<Question> GetAllQuestions()
    {
        return _repo.ReadAllQuestions();
    }
    
    public Flow GetFlowStepsByFlowId(long flowId)
    {
        return _repo.ReadFlowStepsByFlowId(flowId);
    }

    public IEnumerable<FlowStep> GetFlowStepsOfFlow(long flowId)
    {
        return _repo.ReadFlowStepsOfFlow(flowId);
    }
    
    public FlowStep GetFlowStepByOrderNr(int orderNr, long flowId)
    {
        var flowstep =  _repo.ReadFlowStepByOrderNr(orderNr,flowId);
        if (flowstep is Question)
        {
            GetQuestionWithOptions(flowstep);
        }
        
        return flowstep;
    }

    public FlowStep GetQuestionWithOptions(FlowStep flowStep)
    {
        
        if (flowStep is MultipleChoiceQuestion)
        {

            return _repo.ReadFlowStepAsMultipleChoiceQuestion(flowStep);

        }
        else if (flowStep is ClosedQuestion )
        {
            return _repo.ReadFlowStepAsClosedQuestion(flowStep);
        }

        return flowStep;

    }

    public FlowStep IdentifyFlowStep(FlowStep fs)
    {
        return _repo.IdentifyFlowStep(fs);
    }



    public ClosedQuestion AddClosedQuestion(ClosedQuestion question)
    {
        return _repo.CreateClosedQuestion(question);
    }

    public MultipleChoiceQuestion AddMultipleChoiceQuestion(MultipleChoiceQuestion question)
    {
        return _repo.CreateMultipleChoiceQuestion(question);
    }

    public RangeQuestion AddRangeQuestion(RangeQuestion question)
    {
        return _repo.CreateRangeQuestion(question);
    }

    public OpenQuestion AddOpenQuestion(OpenQuestion question)
    {
        return _repo.CreateOpenQuestion(question);
    }



    public ClosedQuestion GetClosedQuestionWithOptions(long flowStepId)
    {
        return _repo.ReadClosedQuestionWithOptions(flowStepId);
    }
    public MultipleChoiceQuestion GetMultipleChoiceQuestionWithOptions(long flowStepId)
    {
        return _repo.ReadMultipleChoiceQuestionWithOptions(flowStepId);
    }
    public void DeleteClosedQuestion(ClosedQuestion question)
    {
            _repo.DeleteClosedQuestion( question);
    }

    public void DeleteMultipleChoiceQuestion(MultipleChoiceQuestion question)
    {
        _repo.DeleteMultipleChoiceQuestion(question);
    }

    public void DeleteRangeQuestion(RangeQuestion question)
    {
        _repo.DeleteRangeQuestion(question);
    }

    public void DeleteOpenQuestion(OpenQuestion question)
    {
        _repo.DeleteOpenQuestion(question);
    }

    public void DeleteInfo(Info info)
    {
        _repo.DeleteInfo(info);
    }

    public FlowStep GetFlowStep(long id)
    {
        
        return _repo.ReadFlowStep(id);
    }

    public  OpenQuestion UpdateOpenQuestion(OpenQuestion question)
    {
     return   _repo.UpdateOpenQuestion(question);
    }

    public ClosedQuestion UpdateClosedQuestion(ClosedQuestion question)
    {
        return  _repo.UpdateClosedQuestion(question);
    }

    public MultipleChoiceQuestion UpdateMultipleChoiceQuestion(MultipleChoiceQuestion question)
    {
        return  _repo.UpdateMultipleChoiceQuestion(question);
    }

    public RangeQuestion UpdateRangeQuestion(RangeQuestion question)
    {
        return  _repo.UpdateRangeQuestion(question);
    }

    public async Task UpdateInfo(Info info)
    {
        await _repo.UpdateInfo(info);
    }
    
    public Option AddOptionToQuestion(Option option, long questionId)
    {
        return _repo.CreateOptionToQuestion(option, questionId);
    }
     
    public void DeleteOptionFromQuestion(Option option, long questionId)
    {
        _repo.DeleteOptionFromQuestion(option, questionId);
    } 
    
    public Option RetrieveOption(long optionId)
    {
        return _repo.ReadOption(optionId);
    }
    
    public IEnumerable<Option> UpdateOptions(IEnumerable<Option> options, long questionId)
    {
        return _repo.UpdateOptions(options, questionId);
    }
    
    public IEnumerable<Option> GetOptionsByQuestionId(long questionId)
    {
        return _repo.ReadOptionsByQuestionId(questionId);
    }
    
    public MultipleChoiceQuestion RetrieveMultipleChoiceQuestionByQuestionId(long questionId)
    {
        return _repo.ReadMultipleChoiceQuestionByQuestionId(questionId);
    }
     
    public ClosedQuestion RetrieveClosedQuestionByQuestionId(long questionId)
    {
        return _repo.ReadClosedQuestionByQuestionId(questionId);
    }

    public FlowStep UpdateFlowStep(FlowStep flowStep)
    {
        return _repo.UpdateFlowStep(flowStep);
    }
    public RangeQuestion GetRangeQuestion(long id)
    {
        return _repo.ReadRangeQuestion(id);
    }
     
    public Option UpdateOption(Option option)
    {
        return _repo.UpdateOption(option);
    }

    

    public Answer AddCriteriaForConditionalPoint(Answer answer)
    {
        return _repo.CreateCriteriaForConditionalPoint(answer);
    }

    public ConditionalPoint AddConditionalPoint(long questionId, FlowStep flowStep, Answer criteria)
    {
        
        var question = _repo.ReadFlowStep(questionId) as Question;
        
        var conditionalPoint = new ConditionalPoint
        {
            Question = question,
            Criteria = criteria,
            FollowUpStep = flowStep
        };
        var followUpStep = flowStep;
        followUpStep.ConditionalPoint = conditionalPoint;
       var flow = _manager.GetFlow(question!.FlowId);
        followUpStep.Flow = flow;
        followUpStep.SubTheme = question.SubTheme;
        followUpStep.IsActive = true;
        followUpStep.OrderNr = _repo.ReadLargestOrderNr(question.FlowId) + 1;


        if (followUpStep is Info info)
        {
            _repo.CreateInfo(info);
        }
        else
        {
            _repo.CreateQuestion(followUpStep as Question);
        }
        conditionalPoint = _repo.CreateConditionalPoint(conditionalPoint);
        criteria = AddCriteriaForConditionalPoint(criteria);

        criteria.ConditionalPoint = conditionalPoint;
        // question.QuestionConditionalPoints.Add(conditionalPoint); 
        //
        // switch (question)
        // {
        //     case ClosedQuestion closedQuestion:
        //         UpdateClosedQuestion(closedQuestion);
        //         break;
        //     case MultipleChoiceQuestion multipleChoiceQuestion:
        //         UpdateMultipleChoiceQuestion(multipleChoiceQuestion);
        //         break;
        //     case OpenQuestion openQuestion:
        //
        //         UpdateOpenQuestion(openQuestion);
        //         break;
        //     case RangeQuestion rangeQuestion:
        //         UpdateRangeQuestion(rangeQuestion);
        //         break;
        // }
        return conditionalPoint;
    }

    public string GetOptionTextForOption(long id)
    {
        return _repo.ReadOptionTextForOption(id);
    }

    public void RemoveConditionalPoint(long conditionalPointId)
    {
        _repo.DeleteConditionalPoint(conditionalPointId);
    }
}