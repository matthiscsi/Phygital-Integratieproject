using PIP.Domain.Flow;
using PIP.Domain.Flow.Inquiry;

namespace PIP.BL.IManagers;

public interface IFlowStepManager
{
    Info AddInfo(Info info);
    Info GetInfo(long id);
    IEnumerable<Info> GetAllInfo();

    Question AddQuestion(Question question);
    Question GetQuestion(long id);
    IEnumerable<Question> GetAllQuestions();

    Flow GetFlowStepsByFlowId(long flowId);
    IEnumerable<FlowStep> GetFlowStepsOfFlow(long flowId);
    FlowStep GetFlowStepByOrderNr(int orderNr, long flowId);

    FlowStep GetQuestionWithOptions(FlowStep flowStep);

    FlowStep IdentifyFlowStep(FlowStep fs);

    FlowStep GetFlowStep(long flowStepId);

    ClosedQuestion AddClosedQuestion(ClosedQuestion question);
    MultipleChoiceQuestion AddMultipleChoiceQuestion(MultipleChoiceQuestion question);
    RangeQuestion AddRangeQuestion(RangeQuestion question);
    OpenQuestion AddOpenQuestion(OpenQuestion question);


    ClosedQuestion GetClosedQuestionWithOptions(long flowStepId);

    MultipleChoiceQuestion GetMultipleChoiceQuestionWithOptions(long flowStepId);

    void DeleteClosedQuestion(ClosedQuestion question);
    void DeleteMultipleChoiceQuestion(MultipleChoiceQuestion question);
    void DeleteRangeQuestion(RangeQuestion question);
    void DeleteOpenQuestion(OpenQuestion question);
    void DeleteInfo(Info info);

    OpenQuestion UpdateOpenQuestion(OpenQuestion question);
    ClosedQuestion UpdateClosedQuestion(ClosedQuestion question);
    MultipleChoiceQuestion UpdateMultipleChoiceQuestion(MultipleChoiceQuestion question);
    RangeQuestion UpdateRangeQuestion(RangeQuestion question);
    Task UpdateInfo(Info info);

    Option AddOptionToQuestion(Option option, long questionId);
    void DeleteOptionFromQuestion(Option option, long questionId);

    Option RetrieveOption(long optionId);

    IEnumerable<Option> UpdateOptions(IEnumerable<Option> options, long questionId);

    IEnumerable<Option> GetOptionsByQuestionId(long questionId);
    MultipleChoiceQuestion RetrieveMultipleChoiceQuestionByQuestionId(long questionId);

    ClosedQuestion RetrieveClosedQuestionByQuestionId(long questionId);

    FlowStep UpdateFlowStep(FlowStep flowStep);
    RangeQuestion GetRangeQuestion(long id);
    Option UpdateOption(Option option);
    ConditionalPoint AddConditionalPoint(long questionId, FlowStep flowStep, Answer criteria);
    Answer AddCriteriaForConditionalPoint(Answer answer);
    string GetOptionTextForOption(long id);
    void RemoveConditionalPoint(long conditionalPointId);
}