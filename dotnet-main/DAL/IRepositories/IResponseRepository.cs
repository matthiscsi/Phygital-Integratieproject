using PIP.Domain.Flow.Inquiry;

namespace PIP.DAL.IRepositories;

public interface IResponseRepository
{
    Response CreateResponse(Response response);
    int ReadResponseCountForClosedOption(long optionId);
    int ReadResponseCountForMultipleChoiceOption(long optionId);
    IEnumerable<OpenAnswer> ReadAnswersForOpenQuestion(long id);
    IEnumerable<RangeAnswer> ReadAnswersForRangeQuestion(long id);
    Answer CreateAnswer(Answer answer);
    int ReadResponseCountForQuestion(long id);
}