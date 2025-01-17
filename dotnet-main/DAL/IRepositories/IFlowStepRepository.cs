using PIP.Domain.Flow;
using PIP.Domain.Flow.Inquiry;

namespace PIP.DAL.IRepositories;

public interface IFlowStepRepository
{
    FlowStep ReadFlowStep(long id);

    Question CreateQuestion(Question question);
    Question ReadQuestion(long id);
    IEnumerable<Question> ReadAllQuestions();
    Info CreateInfo(Info info);
    Info ReadInfo(long id);
    IEnumerable<Info> ReadAllInfo();

    Flow ReadFlowStepsByFlowId(long flowId);
    IEnumerable<FlowStep> ReadFlowStepsOfFlow(long flowId);
    FlowStep ReadFlowStepByOrderNr(int orderNr, long flowid);


    FlowStep IdentifyFlowStep(FlowStep fs);

    ClosedQuestion ReadFlowStepAsClosedQuestion(FlowStep flowStep);
    MultipleChoiceQuestion ReadFlowStepAsMultipleChoiceQuestion(FlowStep flowStep);

    OpenQuestion CreateOpenQuestion(OpenQuestion question);
    ClosedQuestion CreateClosedQuestion(ClosedQuestion question);
    MultipleChoiceQuestion CreateMultipleChoiceQuestion(MultipleChoiceQuestion question);
    RangeQuestion CreateRangeQuestion(RangeQuestion question);
    ClosedQuestion ReadClosedQuestionWithOptions(long flowStepId);
    MultipleChoiceQuestion ReadMultipleChoiceQuestionWithOptions(long flowStepId);
    void DeleteOpenQuestion(OpenQuestion question);
    void DeleteClosedQuestion(ClosedQuestion question);
    void DeleteMultipleChoiceQuestion(MultipleChoiceQuestion question);
    void DeleteRangeQuestion(RangeQuestion question);
    void DeleteInfo(Info info);
    OpenQuestion UpdateOpenQuestion(OpenQuestion question);
    ClosedQuestion UpdateClosedQuestion(ClosedQuestion question);
    MultipleChoiceQuestion UpdateMultipleChoiceQuestion(MultipleChoiceQuestion question);
    RangeQuestion UpdateRangeQuestion(RangeQuestion question);
    Task UpdateInfo(Info info); 
    Option CreateOptionToQuestion(Option option, long questionId);
    void DeleteOptionFromQuestion(Option option, long questionId);
    Option ReadOption(long optionId);
    IEnumerable<Option> UpdateOptions(IEnumerable<Option> options, long questionId);
    IEnumerable<Option> ReadOptionsByQuestionId(long questionId);
    MultipleChoiceQuestion ReadMultipleChoiceQuestionByQuestionId(long questionId);
    ClosedQuestion ReadClosedQuestionByQuestionId(long questionId);
    FlowStep UpdateFlowStep(FlowStep flowStep);
    RangeQuestion ReadRangeQuestion(long id);
    Option UpdateOption(Option option);
    Answer GetAnswerForQuestion(long questionFlowStepId, long flowSessionId);
    Answer ReadCriteriaForConditionalPoint(long conditionalPointId);

    FlowStep ReadFlowStepWithConditionalPoint(long? flowStepId);
    ConditionalPoint CreateConditionalPoint(ConditionalPoint conditionalPoint);
    Answer CreateCriteriaForConditionalPoint(Answer answer);
     Question CreateConditionalPointToQuestion(long questionId, long conditionalPointId);
     int ReadLargestOrderNr(long flowId);


     string ReadOptionTextForOption(long id);
     void DeleteConditionalPoint(long conditionalPointId);
}