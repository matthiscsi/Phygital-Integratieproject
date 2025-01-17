using PIP.BL.IManagers;
using PIP.DAL.IRepositories;
using Pip.Domain.Flow;
using PIP.Domain.Flow.Inquiry;

namespace PIP.BL.Managers;

public class ResponseManager : IResponseManager
{
    private readonly IResponseRepository _responseRepository;

    public ResponseManager(IResponseRepository responseRepository)
    {
        _responseRepository = responseRepository;
    }

    public Response AddResponse(Answer answer, Question question, FlowSession flowSession)
    {
        var response = new Response { Answer = answer, Question = question, FlowSession = flowSession };
        _responseRepository.CreateAnswer(answer);
        var responseSaved = _responseRepository.CreateResponse(response);
        return responseSaved;

    }

    public int GetResponseCountForClosedOption(long optionId)
    {
        return _responseRepository.ReadResponseCountForClosedOption(optionId);
    }

    public int GetResponseCountForMultipleChoiceOption(long optionId)
    {
        return _responseRepository.ReadResponseCountForMultipleChoiceOption(optionId);
    }

    public IEnumerable<OpenAnswer> GetAnswersForOpenQuestion(long id)
    {
        return _responseRepository.ReadAnswersForOpenQuestion(id);
    }

    public IEnumerable<RangeAnswer> GetAnswersForRangeQuestion(long id)
    {
        return _responseRepository.ReadAnswersForRangeQuestion(id);
    }

    public int GetResponseCountForQuestion(long id)
    {
        return _responseRepository.ReadResponseCountForQuestion(id);
    }
}