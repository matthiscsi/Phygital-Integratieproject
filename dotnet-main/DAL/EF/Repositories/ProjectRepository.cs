using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PIP.DAL.IRepositories;

using PIP.Domain.Deelplatform;
using PIP.Domain.Flow;
using PIP.Domain.Flow.Inquiry;
using PIP.Domain.User;

namespace PIP.DAL.EF.Repositories;

public class ProjectRepository : IProjectRepository
{
    private readonly PhygitalDbContext _context;
    private readonly IFlowRepository _flowRepository;

    public ProjectRepository(PhygitalDbContext context, IFlowRepository flowRepository)
    {
        _flowRepository = flowRepository;
        _context = context;
    }

    public IEnumerable<Flow> ReadFlowsOfProject(long id)
    {
        var project = ReadProject(id);
        return _flowRepository.ReadAllFlows()
            .Where(f => f.Project == project).AsQueryable();
    }

    public Project ReadProject(long id)
    {
        return _context.Projects.Include(p => p.SubPlatformAdmin).Include(p => p.Flows).Include(p=>p.Subplatform)
            .SingleOrDefault(project => project.ProjectId == id);
    }

    public IEnumerable<Project> ReadActiveProjects(SubPlatformAdministrator user)
    {
        return _context.Projects
            .Where(project => project.IsActive && project.SubPlatformAdmin == user)
            .Include(project => project.Subplatform)
            .AsQueryable();
    }

    public IEnumerable<Project> ReadProjectsOfUser(IdentityUser user)
    {

        var userEmployee = user as SubPlatformAdministrator;
        var userSubplatform = _context.Projects
            .Where(project => project.Subplatform.Equals(userEmployee.Subplatform))
            .Select(p => p.Subplatform)
            .FirstOrDefault();


        return _context.Projects
            .Where(project => project.Subplatform.Equals(userSubplatform))
            .Include(p => p.Flows)
            .ThenInclude(flow => flow.Theme)
            .Include(p => p.Subplatform)
            .AsQueryable();
    }
    


 public Project DeleteProject(long id)
{
    var project = ReadProject(id);

    var flowsToDelete = _context.Flows
        .Where(f => f.Project.ProjectId == id)
        .ToList();

    foreach (var flow in flowsToDelete)
    {
        // Verwijder alle flowsteps van de flow
        var flowStepsToDelete = _context.FlowSteps
            .Where(fs => fs.FlowId == flow.FlowId)
            .ToList();

        foreach (var flowStep in flowStepsToDelete)
        {
            if (flowStep is Question question)
            {
                DeleteQuestion(question);
            }

            _context.FlowSteps.Remove(flowStep);
        }
    }

    // Verwijder de flows
    _context.Flows.RemoveRange(flowsToDelete);

    // Verwijder het project
    _context.Projects.Remove(project);

    _context.SaveChanges();

    return project;
}

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


    public Project CreateProject(Project project)
    {
        _context.Projects.Add(project);
        _context.SaveChanges();
        return project;
    }

    public Project UpdateProject(Project project)
    {
        _context.Projects.Update(project);
        _context.SaveChanges();
        return project;
    }

    public Project ReadProjectWithSubplatform(long id)
    {
        return _context.Projects.Include(p => p.Subplatform).SingleOrDefault(project => project.ProjectId == id);
    }

    public Project ReadProjectFromSubtheme(long id)
    {
        return _context.Projects.Include(p=>p.SubPlatformAdmin).Include(p => p.Flows).ThenInclude(f => f.Theme.SubThemes).SingleOrDefault(project =>
            project.Flows.Any(flow => flow.Theme.SubThemes.Any(subtheme => subtheme.SubthemeId == id)));
    }

    public Project ReadProjectFromFlow(long id)
    {
        return _context.Projects.Include(p=>p.SubPlatformAdmin).Include(p => p.Flows).SingleOrDefault(project =>
            project.Flows.Any(flow => flow.FlowId == id));
    }

    public Project ReadProjectFromFlowstep(long id)
    {
        return _context.Projects.Include(p=>p.SubPlatformAdmin).Include(p => p.Flows).SingleOrDefault(project =>
            project.Flows.Any(flow => flow.FlowSteps.Any(flowStep => flowStep.FlowStepId == id)));
    }

    public IEnumerable<Project> ReadProjectsFromSubplatform(long id)
    {
        return _context.Projects
            .Where(project => project.Subplatform.SubplatformId == id)
            .Include(p => p.Flows)
            .ThenInclude(flow => flow.Theme)
            .Include(p => p.Subplatform);
    }
}