using Microsoft.EntityFrameworkCore;
using PIP.DAL.IRepositories;
using PIP.Domain.Flow.Inquiry;

namespace PIP.DAL.EF.Repositories;

public class ResponseRepository : IResponseRepository
{
    private readonly PhygitalDbContext _context;

    public ResponseRepository(PhygitalDbContext dbContext)
    {
        _context = dbContext;
    }

    public Response CreateResponse(Response response)
    {
        _context.Responses.Add(response);
        _context.SaveChanges();

        return response;
    }

    public int ReadResponseCountForClosedOption(long optionId)
    {
        return _context.ClosedAnswers.Count(a => a.SelectedAnswer == optionId);
    }

    public int ReadResponseCountForMultipleChoiceOption(long optionId)
    {
        //sadly werkt dit niet anders dan met client side door JSON property 'selectedAnswers'
        return _context.MultipleChoiceAnswers
            .AsEnumerable()  
            .Count(a => a.SelectedAnswers.Contains(optionId));
    }

    public IEnumerable<OpenAnswer> ReadAnswersForOpenQuestion(long id)
    {
        var answers = new List<OpenAnswer>();
        
        var responses = _context.Responses.Include(r=>r.Answer).Include(r=>r.FlowSession).Where(r => r.Question.FlowStepId == id);
        foreach (Response response in responses)
        {
            answers.Add((OpenAnswer)response.Answer);
        }

        return answers;
    }

    public IEnumerable<RangeAnswer> ReadAnswersForRangeQuestion(long id)
    {
        var answers = new List<RangeAnswer>();
        
        var responses = _context.Responses.Include(r=>r.Answer).Where(r => r.Question.FlowStepId == id);
        foreach (Response response in responses)
        {
            answers.Add((RangeAnswer)response.Answer);
        }
        return answers;
    }

    public Answer CreateAnswer(Answer answer)
    {
        _context.Answers.Add(answer);
        _context.SaveChanges();
        return answer;
    }

    public int ReadResponseCountForQuestion(long id)
    {
        return _context.Responses.Count(r => r.Question.FlowStepId == id);
    }
}