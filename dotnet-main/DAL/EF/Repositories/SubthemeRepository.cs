using Microsoft.EntityFrameworkCore;
using PIP.DAL.IRepositories;
using PIP.Domain.Flow;
using PIP.Domain.Flow.Inquiry;

namespace PIP.DAL.EF.Repositories;

public class SubthemeRepository : ISubthemeRepository
{

    private readonly PhygitalDbContext _context;

    public SubthemeRepository(PhygitalDbContext dbContext)
    {
        _context = dbContext;
    }

    public Subtheme CreateSubtheme(Subtheme subtheme)
    {
        _context.Subthemes.Add(subtheme);
        _context.SaveChanges();

        return subtheme;
    }

    public Subtheme ReadSubtheme(long id)
    {
        return _context.Subthemes.Find(id);
    }

public Subtheme DeleteSubtheme(long id)
{
    using var transaction = _context.Database.BeginTransaction();
    try
    {
        var subtheme = _context.Subthemes.Include(st => st.ParentTheme).FirstOrDefault(st => st.SubthemeId == id);

        if (subtheme == null) return null;

        var flowSteps = _context.FlowSteps.Where(fs => fs.SubTheme == subtheme);
        foreach (var flowStep in flowSteps)
        {
            // Delete related entities based on the type of flow step
            if (flowStep is Question question)
            {
                DeleteQuestion(question);
            }
            else if (flowStep is Info info)
            {
                _context.FlowSteps.Remove(info);
            }
        }

        // Remove the subtheme from its parent theme
        var parentTheme = _context.Themes.Include(t => t.SubThemes)
            .FirstOrDefault(t => t.SubThemes.Any(st => st.SubthemeId == id));

        if (parentTheme != null)
        {
            var subthemeToRemove = parentTheme.SubThemes.FirstOrDefault(st => st.SubthemeId == id);
            if (subthemeToRemove != null)
            {
                parentTheme.SubThemes = parentTheme.SubThemes.Where(st => st != subthemeToRemove).ToList();
            }
        }


        // Remove the subtheme itself
        _context.Subthemes.Remove(subtheme);
        _context.SaveChanges();

        // Commit the transaction
        transaction.Commit();

        return subtheme;
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

// Method to delete a question and its related entities
private void DeleteQuestion(Question question)
{
    var responsesToDelete = _context.Responses
        .Where(r => r.Question == question)
        .ToList();
    _context.Responses.RemoveRange(responsesToDelete);

    var flowSessionsToDelete = _context.FlowSessions
        .Where(fs => fs.Responses.Any(r => r.Question == question))
        .ToList();
    _context.FlowSessions.RemoveRange(flowSessionsToDelete);

    foreach (var conditionalPoint in question.QuestionConditionalPoints)
    {
        _context.ConditionalPoints.Remove(conditionalPoint);
    }

    if (question is ClosedQuestion closedQuestion)
    {
        var closedQuestionToDelete = _context.ClosedQuestions
            .Where(cq => cq.FlowStepId == closedQuestion.FlowStepId)
            .Include(cq => cq.Options)
            .ThenInclude(option => option.Question)
            .Include(question => question.QuestionConditionalPoints)
            .FirstOrDefault();

        if (closedQuestionToDelete != null)
        {
            foreach (var option in closedQuestionToDelete.Options)
            {
                if (option != null)
                {
                    if (option.Question != null)
                    {
                        _context.Questions.Remove(option.Question);
                    }
                    _context.Options.Remove(option);
                }
            }
        }
    }
    else if (question is MultipleChoiceQuestion multipleChoiceQuestion)
    {
        var multipleChoiceQuestionToDelete = _context.MultipleChoiceQuestions
            .Where(mcq => mcq.FlowStepId == multipleChoiceQuestion.FlowStepId)
            .Include(mcq => mcq.Options)
            .ThenInclude(option => option.Question)
            .FirstOrDefault();

        if (multipleChoiceQuestionToDelete != null)
        {
            foreach (var option in multipleChoiceQuestionToDelete.Options)
            {
                if (option != null)
                {
                    if (option.Question != null)
                    {
                        _context.Questions.Remove(option.Question);
                    }
                    _context.Options.Remove(option);
                }
            }
        }
    }

    _context.Questions.Remove(question);
}

    
    public Subtheme UpdateSubtheme(Subtheme subtheme)
    {
        _context.Subthemes.Update(subtheme);
        _context.SaveChanges();

        return subtheme;
    }
}
