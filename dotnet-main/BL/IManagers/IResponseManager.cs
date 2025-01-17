using Pip.Domain.Flow;
using PIP.Domain.Flow.Inquiry;

namespace PIP.BL.IManagers;

public interface IResponseManager
{
    Response AddResponse(Answer answer, Question question, FlowSession flowSession);
    int GetResponseCountForClosedOption(long optionId);
    int GetResponseCountForMultipleChoiceOption(long optionId);

    IEnumerable<OpenAnswer> GetAnswersForOpenQuestion(long id);
    IEnumerable<RangeAnswer> GetAnswersForRangeQuestion(long id);
    int GetResponseCountForQuestion(long id);
}