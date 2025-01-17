using Microsoft.EntityFrameworkCore;
using PIP.DAL.IRepositories;
using PIP.Domain.Flow;
using PIP.Domain.Flow.Inquiry;


namespace PIP.DAL.EF.Repositories;

public class FlowStepRepository : IFlowStepRepository
{
    private readonly PhygitalDbContext _context;

    public FlowStepRepository(PhygitalDbContext context)
    {
        _context = context;
    }

    public FlowStep ReadFlowStep(long id)
    {
        return _context.FlowSteps.Include(fs => fs.Flow).Where(fs => fs.FlowStepId == id).Include(fs => fs.SubTheme)
            .SingleOrDefault();
    }


    public Question CreateQuestion(Question question)
    {
        _context.Questions.Add(question);
        _context.SaveChanges();
        return question;
    }

    public Question ReadQuestion(long id)
    {
        return _context.Questions.Where(q => q.FlowStepId == id).Include(q => q.SubTheme)
            .Include(fs => fs.QuestionConditionalPoints)
            .ThenInclude(cp => cp.Criteria).Include(fs => fs.QuestionConditionalPoints)
            .ThenInclude(cp => cp.FollowUpStep)
            .SingleOrDefault();
    }

    public IEnumerable<Question> ReadAllQuestions()
    {
        return _context.Questions;
    }

    public Info CreateInfo(Info info)
    {
        _context.Infos.Add(info);
        _context.SaveChanges();
        return info;
    }

    public Info ReadInfo(long id)
    {
        return _context.Infos.Where(i => i.FlowStepId == id).Include(i => i.SubTheme).SingleOrDefault();
    }

    public IEnumerable<Info> ReadAllInfo()
    {
        return _context.Infos;
    }

    public Flow ReadFlowStepsByFlowId(long flowId)
    {
        var flow = _context.Flows
            .Where(f => f.FlowId == flowId)
            .Include(f => f.FlowSteps.OrderBy(fs => fs.OrderNr))
            .ThenInclude(fs => fs.SubTheme)
            .FirstOrDefault();

        return flow;
    }

    public IEnumerable<FlowStep> ReadFlowStepsOfFlow(long flowId)
    {
        var flowsteps = _context.Flows
            .Where(flow => flow.FlowId == flowId).Include(flow => flow.FlowSteps)
            .SingleOrDefault()!.FlowSteps;

        return flowsteps;
    }

    public FlowStep ReadFlowStepByOrderNr(int orderNr, long flowid)
    {
        var flow = _context.Flows
            .Where(f => f.FlowId == flowid)
            .Include(f => f.FlowSteps)
            .ThenInclude(fs => fs.SubTheme)
            .FirstOrDefault();

        return flow?.FlowSteps.FirstOrDefault(f => f.OrderNr == orderNr);
    }

    public ClosedQuestion ReadFlowStepAsClosedQuestion(FlowStep flowStep)
    {
        return _context.ClosedQuestions
            .Include(q => q.Options).Include(q => q.SubTheme)
            .Include(fs => fs.QuestionConditionalPoints)
            .ThenInclude(cp => cp.Criteria)
            .Include(fs => fs.QuestionConditionalPoints)
            .ThenInclude(cp => cp.FollowUpStep)
            .FirstOrDefault(q => q.FlowStepId == flowStep.FlowStepId);
    }

    public MultipleChoiceQuestion ReadFlowStepAsMultipleChoiceQuestion(FlowStep flowStep)
    {
        return _context.MultipleChoiceQuestions
            .Include(q => q.Options).Include(q => q.SubTheme)
            .Include(fs => fs.QuestionConditionalPoints)
            .ThenInclude(cp => cp.Criteria)
            .Include(fs => fs.QuestionConditionalPoints)
            .ThenInclude(cp => cp.FollowUpStep)
            .FirstOrDefault(q => q.FlowStepId == flowStep.FlowStepId);
    }

    public RangeQuestion ReadFlowStepAsRangeQuestion(FlowStep flowStep)
    {
        return _context.RangeQuestions.Include(q => q.SubTheme).Include(fs => fs.QuestionConditionalPoints)
            .ThenInclude(cp => cp.Criteria)
            .Include(fs => fs.QuestionConditionalPoints).ThenInclude(cp => cp.FollowUpStep)
            .FirstOrDefault(q => q.FlowStepId == flowStep.FlowStepId);
    }

    public OpenQuestion CreateOpenQuestion(OpenQuestion question)
    {
        _context.OpenQuestions.Add(question);
        _context.SaveChanges();
        return question;
    }

    public ClosedQuestion CreateClosedQuestion(ClosedQuestion question)
    {
        _context.ClosedQuestions.Add(question);
        _context.SaveChanges();
        return question;
    }

    public MultipleChoiceQuestion CreateMultipleChoiceQuestion(MultipleChoiceQuestion question)
    {
        _context.MultipleChoiceQuestions.Add(question);
        _context.SaveChanges();
        return question;
    }

    public RangeQuestion CreateRangeQuestion(RangeQuestion question)
    {
        _context.RangeQuestions.Add(question);
        _context.SaveChanges();
        return question;
    }

  public void DeleteOpenQuestion(OpenQuestion question)
{
    using var transaction = _context.Database.BeginTransaction();
    try
    {
        // Delete FlowSessions associated with responses to the question
        var flowSessionsToDelete = _context.FlowSessions
            .Where(fs => fs.Responses.Any(r => r.Question == question))
            .ToList();
        _context.FlowSessions.RemoveRange(flowSessionsToDelete);

        // Delete responses to the question
        var responsesToDelete = _context.Responses
            .Where(r => r.Question == question)
            .ToList();
        _context.Responses.RemoveRange(responsesToDelete);

        // Delete conditional points related to the question
        var conditionalPointsToDelete = question.QuestionConditionalPoints.ToList();
        _context.ConditionalPoints.RemoveRange(conditionalPointsToDelete);

        // Delete the FlowStep related to the question from its Flow
        var flow = _context.Flows
            .Include(f => f.FlowSteps)
            .FirstOrDefault(f => f.FlowSteps.Any(fs => fs.FlowStepId == question.FlowStepId));

        if (flow != null)
        {
            var flowStepToDelete = flow.FlowSteps.FirstOrDefault(fs => fs.FlowStepId == question.FlowStepId);
            if (flowStepToDelete != null)
            {
                // Create a list from FlowSteps to use the Remove method
                var flowStepsList = flow.FlowSteps.ToList();
                flowStepsList.Remove(flowStepToDelete);
                flow.FlowSteps = flowStepsList;
                // If the relationship between Flow and FlowStep is handled via a foreign key,
                // ensure the removed FlowStep is also removed from the context.
                _context.FlowSteps.Remove(flowStepToDelete);
            }
        }

        // Delete the question itself
        _context.OpenQuestions.Remove(question);

        // Save all changes to the database
        _context.SaveChanges();

        // Commit the transaction
        transaction.Commit();
    }
    catch (Exception ex)
    {
        // Rollback the transaction if any error occurs
        transaction.Rollback();
        // Log the exception
        Console.WriteLine(ex.ToString());
        throw;
    }
}



  public void DeleteClosedQuestion(ClosedQuestion question)
  {
      using var transaction = _context.Database.BeginTransaction();
      try
      {
          // Delete FlowSessions associated with responses to the question
          var flowSessionsToDelete = _context.FlowSessions
              .Where(fs => fs.Responses.Any(r => r.Question == question))
              .ToList();
          _context.FlowSessions.RemoveRange(flowSessionsToDelete);

          // Delete responses to the question
          var responsesToDelete = _context.Responses
              .Where(r => r.Question == question)
              .ToList();
          _context.Responses.RemoveRange(responsesToDelete);

          // Include Options and their related Questions
          var closedQuestionToDelete = _context.ClosedQuestions
              .Where(m => m.FlowStepId == question.FlowStepId)
              .Include(c => c.Options).ThenInclude(option => option.Question)
              .SingleOrDefault();

          // Check if closedQuestionToDelete is not null
          if (closedQuestionToDelete != null)
          {
              // Delete related Options and their Questions
              foreach (var option in closedQuestionToDelete.Options)
              {
                  if (option != null && option.Question != null)
                  {
                      _context.Questions.Remove(option.Question);
                  }
                  _context.Options.Remove(option);
              }

              // Delete the ClosedQuestion
              _context.ClosedQuestions.Remove(closedQuestionToDelete);

              // Save all changes
              _context.SaveChanges();

              // Commit the transaction
              transaction.Commit();
          }
      }
      catch (Exception ex)
      {
          // Rollback the transaction if any error occurs
          transaction.Rollback();
          // Log the exception
          Console.WriteLine(ex.ToString());
          throw;
      }
  }


  public void DeleteMultipleChoiceQuestion(MultipleChoiceQuestion question)
  {
      using var transaction = _context.Database.BeginTransaction();
      try
      {
          // Delete FlowSessions associated with responses to the question
          var flowSessionsToDelete = _context.FlowSessions
              .Where(fs => fs.Responses.Any(r => r.Question == question))
              .ToList();
          _context.FlowSessions.RemoveRange(flowSessionsToDelete);

          // Delete responses to the question
          var responsesToDelete = _context.Responses
              .Where(r => r.Question == question)
              .ToList();
          _context.Responses.RemoveRange(responsesToDelete);

          // Include Options and their related Questions
          var multipleChoiceQuestionToDelete = _context.MultipleChoiceQuestions
              .Where(m => m.FlowStepId == question.FlowStepId)
              .Include(mcq => mcq.Options).ThenInclude(option => option.Question)
              .SingleOrDefault();

          // Check if multipleChoiceQuestionToDelete is not null
          if (multipleChoiceQuestionToDelete != null)
          {
              // Delete related Options and their Questions
              foreach (var option in multipleChoiceQuestionToDelete.Options)
              {
                  if (option != null && option.Question != null)
                  {
                      _context.Questions.Remove(option.Question);
                  }
                  _context.Options.Remove(option);
              }

              // Delete the MultipleChoiceQuestion
              _context.MultipleChoiceQuestions.Remove(multipleChoiceQuestionToDelete);

              // Save all changes
              _context.SaveChanges();

              // Commit the transaction
              transaction.Commit();
          }
      }
      catch (Exception ex)
      {
          // Rollback the transaction if any error occurs
          transaction.Rollback();
          // Log the exception
          Console.WriteLine(ex.ToString());
          throw;
      }
  }

  public void DeleteRangeQuestion(RangeQuestion question)
  {
      using var transaction = _context.Database.BeginTransaction();
      try
      {
          // Delete FlowSessions associated with responses to the question
          var flowSessionsToDelete = _context.FlowSessions
              .Where(fs => fs.Responses.Any(r => r.Question == question))
              .ToList();
          _context.FlowSessions.RemoveRange(flowSessionsToDelete);

          // Delete responses to the question
          var responsesToDelete = _context.Responses
              .Where(r => r.Question == question)
              .ToList();
          _context.Responses.RemoveRange(responsesToDelete);

          // Delete conditional points related to the question
          var conditionalPointsToDelete = question.QuestionConditionalPoints.ToList();
          _context.ConditionalPoints.RemoveRange(conditionalPointsToDelete);

          // Delete the RangeQuestion itself
          _context.RangeQuestions.Remove(question);

          // Save all changes
          _context.SaveChanges();

          // Commit the transaction
          transaction.Commit();
      }
      catch (Exception ex)
      {
          // Rollback the transaction if any error occurs
          transaction.Rollback();
          // Log the exception
          Console.WriteLine(ex.ToString());
          throw;
      }
  }


    public void DeleteInfo(Info info)
    {
        _context.Infos.Remove(info);
        _context.SaveChanges();
    }

    public OpenQuestion UpdateOpenQuestion(OpenQuestion question)
    {
        _context.OpenQuestions.Update(question);

        _context.SaveChanges();
        return question;
    }

    public ClosedQuestion UpdateClosedQuestion(ClosedQuestion question)
    {
        _context.ClosedQuestions.Update(question);
        _context.SaveChanges();
        return question;
    }

    public MultipleChoiceQuestion UpdateMultipleChoiceQuestion(MultipleChoiceQuestion question)
    {
        _context.MultipleChoiceQuestions.Update(question);
        _context.SaveChanges();

        return question;
    }


    public RangeQuestion UpdateRangeQuestion(RangeQuestion question)
    {
        _context.RangeQuestions.Update(question);
        _context.SaveChanges();
        return question;
    }

    public async Task UpdateInfo(Info info)
    {
        _context.Infos.Update(info);
        await _context.SaveChangesAsync();
    }

    public Option CreateOptionToQuestion(Option option, long questionId)
    {
        var question = _context.Questions.Find(questionId);
        if (question == null)
        {
            return null;
        }

        if (question is ClosedQuestion closedQuestion)
        {
            closedQuestion.Options.Add(option);
            _context.ClosedQuestions.Update(closedQuestion);
        }
        else if (question is MultipleChoiceQuestion multipleChoiceQuestion)
        {
            multipleChoiceQuestion.Options.Add(option);
            _context.MultipleChoiceQuestions.Update(multipleChoiceQuestion);
        }


        _context.SaveChanges();
        return option;
    }

    public void DeleteOptionFromQuestion(Option option, long questionId)
    {
        var question = _context.Questions.Find(questionId);
        if (question == null)
        {
            return;
        }

        if (question is ClosedQuestion closedQuestion)
        {
            closedQuestion.Options.Remove(option);
            _context.ClosedQuestions.Update(closedQuestion);
        }
        else if (question is MultipleChoiceQuestion multipleChoiceQuestion)
        {
            multipleChoiceQuestion.Options.Remove(option);
            _context.MultipleChoiceQuestions.Update(multipleChoiceQuestion);
        }

        _context.SaveChanges();
    }

    public Option ReadOption(long optionId)
    {
        return _context.Options.Find(optionId);
    }

    public FlowStep IdentifyFlowStep(FlowStep fs)
    {
        if (fs is MultipleChoiceQuestion mcq)
            return ReadFlowStepAsMultipleChoiceQuestion(mcq);
        if (fs is ClosedQuestion cq)
            return ReadFlowStepAsClosedQuestion(cq);
        if (fs is Info info)
            return ReadInfo(info.FlowStepId);
        if (fs is OpenQuestion openQuestion)
            return ReadQuestion(openQuestion.FlowStepId);
        if (fs is RangeQuestion rangeQuestion) return ReadFlowStepAsRangeQuestion(rangeQuestion);

        return null;
    }

    public IEnumerable<Option> UpdateOptions(IEnumerable<Option> options, long questionId)
    {
        var question = _context.Questions.Find(questionId);
        if (question == null)
        {
            return null;
        }

        if (question is ClosedQuestion closedQuestion)
        {
            closedQuestion.Options = options.ToList();
            _context.ClosedQuestions.Update(closedQuestion);
        }
        else if (question is MultipleChoiceQuestion multipleChoiceQuestion)
        {
            multipleChoiceQuestion.Options = options.ToList();
            _context.MultipleChoiceQuestions.Update(multipleChoiceQuestion);
        }

        foreach (var option in options)
        {
            _context.Options.Add(option);
        }

        _context.SaveChanges();
        return options;
    }

    public IEnumerable<Option> ReadOptionsByQuestionId(long questionId)
    {
        var options = _context.Options.Where(q => q.Question.FlowStepId == questionId).Include(q => q.Question);


        return options.ToList();
    }

    public MultipleChoiceQuestion ReadMultipleChoiceQuestionByQuestionId(long questionId)
    {
        var question = _context.MultipleChoiceQuestions
            .Include(q => q.Options)
            .Include(q => q.SubTheme)
            .FirstOrDefault(q => q.FlowStepId == questionId);

        return question;
    }

    public ClosedQuestion ReadClosedQuestionByQuestionId(long questionId)
    {
        var question = _context.ClosedQuestions
            .Include(q => q.Options)
            .Include(q => q.SubTheme)
            .FirstOrDefault(q => q.FlowStepId == questionId);

        return question;
    }


    public FlowStep UpdateFlowStep(FlowStep flowStep)
    {
        _context.FlowSteps.Update(flowStep);
        _context.SaveChanges();
        return flowStep;
    }

    public ClosedQuestion ReadClosedQuestionWithOptions(long flowStepId)
    {
        return _context.ClosedQuestions.Include(q => q.Options).Single(cq => cq.FlowStepId == flowStepId);
    }

    public MultipleChoiceQuestion ReadMultipleChoiceQuestionWithOptions(long flowStepId)
    {
        return _context.MultipleChoiceQuestions.Include(q => q.Options).Single(cq => cq.FlowStepId == flowStepId);
    }

    public RangeQuestion ReadRangeQuestion(long id)
    {
        return _context.RangeQuestions.SingleOrDefault(rq => rq.FlowStepId == id);
    }

    public Option UpdateOption(Option option)
    {
        _context.Options.Update(option);
        _context.SaveChanges();
        return option;
    }

    public Answer GetAnswerForQuestion(long questionFlowStepId, long flowSessionId)
    {
        Question question = _context.Questions.SingleOrDefault(q => q.FlowStepId == questionFlowStepId);
        if (question == null)
        {
            return null;
        }

        var responses = _context.Responses
            .Include(r => r.FlowSession)
            .Include(r => r.Answer)
            .Where(r => r.Question == question);

        var responseOfFlowsession = responses
            .Include(r => r.Answer)
            .SingleOrDefault(r => r.FlowSession.FlowSessionId == flowSessionId);

        if (responseOfFlowsession == null)
        {
            return null;
        }

        return responseOfFlowsession.Answer;
    }

    public Answer ReadCriteriaForConditionalPoint(long conditionalPointId)
    {
        ConditionalPoint cp = _context.ConditionalPoints.Include(cp => cp.Criteria)
            .Single(cp => cp.ConditionalPointId == conditionalPointId);
        return cp.Criteria;
    }

    public FlowStep ReadFlowStepWithConditionalPoint(long? flowStepId)
    {
        return _context.FlowSteps.Include(fs => fs.ConditionalPoint).Single(fs => fs.FlowStepId == flowStepId);
    }

    public ConditionalPoint CreateConditionalPoint(ConditionalPoint conditionalPoint)
    {
        _context.ConditionalPoints.Add(conditionalPoint);
        return conditionalPoint;
    }

    public Answer CreateCriteriaForConditionalPoint(Answer answer)
    {
        _context.Answers.Add(answer);
        return answer;
    }

    public Question CreateConditionalPointToQuestion(long questionId, long conditionalPointId)
    {
        var question = _context.Questions.SingleOrDefault(q => q.FlowStepId == questionId);
        var conditionalpoint =
            _context.ConditionalPoints.SingleOrDefault(cp => cp.ConditionalPointId == conditionalPointId);
        question?.QuestionConditionalPoints.Add(conditionalpoint);
        _context.SaveChanges();
        return question;
    }

    public int ReadLargestOrderNr(long flowId)
    {
        var flowSteps = _context.FlowSteps.Where(fs => fs.FlowId == flowId);
        int largestOrderNr = flowSteps.Max(fs => fs.OrderNr);
        return largestOrderNr;
    }

    public string ReadOptionTextForOption(long id)
    {
        return _context.Options.SingleOrDefault(q => q.Id == id)?.Text;
    }

    public void DeleteConditionalPoint(long conditionalPointId)
    {
        var conditionalPoint = _context.ConditionalPoints.Include(cp=>cp.Criteria).ThenInclude(a=>a.Response).Include(cp => cp.FollowUpStep).FirstOrDefault(cp => cp.ConditionalPointId == conditionalPointId);
        if (conditionalPoint == null)
        {
            return;
        }

        if (conditionalPoint.Criteria.Response != null)
        {
            _context.Responses.Remove(conditionalPoint.Criteria.Response);
        }
        if (conditionalPoint.Criteria != null)
        {
            _context.Answers.Remove(conditionalPoint.Criteria);
        }
        if (conditionalPoint.FollowUpStep != null)
        {
            _context.FlowSteps.Remove(conditionalPoint.FollowUpStep);
        }
        _context.ConditionalPoints.Remove(conditionalPoint);
        _context.SaveChanges();
        
    }
}