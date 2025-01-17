using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.AspNetCore.Identity;
using PIP.BL.IManagers;
using PIP.DAL.IRepositories;
using PIP.Domain.Deelplatform;
using PIP.Domain.User;


namespace PIP.BL.Managers;

public class ProjectManager : IProjectManager
{
    private readonly IProjectRepository _projectRepository;
    private readonly IFlowSessionManager _flowSessionManager;

    public ProjectManager(IProjectRepository projectRepository, IFlowSessionManager flowSessionManager)
    {
        _projectRepository = projectRepository;
        _flowSessionManager = flowSessionManager;
    }


    public Project GetProject(long id)
    {
        return _projectRepository.ReadProject(id);
    }

    public IEnumerable<Project> GetActiveProjects(SubPlatformAdministrator user)
    {
        return _projectRepository.ReadActiveProjects(user);
    }

    public IEnumerable<Project> GetProjectsOfUser(IdentityUser user)
    {
        return _projectRepository.ReadProjectsOfUser(user);
    }


    public Project DeleteProject(long id)
    {
        var deletedProject = _projectRepository.DeleteProject(id);
        return deletedProject;
    }



    public Project AddProject(Project project)
    {
    
        List<ValidationResult> errors = new List<ValidationResult>();
        bool isValid = Validator.TryValidateObject(project, new ValidationContext(project), errors, validateAllProperties: true);

        if (!isValid)
        {
            StringBuilder sb = new StringBuilder();
            foreach (ValidationResult validationResult in errors)
            {
                sb.Append("|"+validationResult.ErrorMessage);
            }
            throw new ValidationException(sb.ToString());
        }

       
        return _projectRepository.CreateProject(project);
    }
     
    public Project UpdateProject(Project project)
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(project);
        if (!Validator.TryValidateObject(project, validationContext, validationResults, true))
        {
            var errors = validationResults.Select(vr => vr.ErrorMessage);
            throw new ValidationException("Project validation failed: " + string.Join(", ", errors));
        }

        return _projectRepository.UpdateProject(project);
    }

    public int GetFlowSessionCountOfProject(long projectId)
    {
        Project project = _projectRepository.ReadProject(projectId);
        int flowSessionCount = 0;
        foreach (var flow in project.Flows)
        {
           flowSessionCount += _flowSessionManager.GetFlowSessionCount(flow.FlowId);
        }

        return flowSessionCount;
    }

    public int GetAverageTimeSpentForFlowsOfProject(long projectId)
    {
        Project project = _projectRepository.ReadProject(projectId);
        int averageTime = 0;
        foreach (var flow in project.Flows)
        {
            averageTime += _flowSessionManager.GetAverageTimeSpentForFlow(flow.FlowId);
        }

        return averageTime;
    }



    public Project GetProjectWithSubplatform(long id)
    {
        return _projectRepository.ReadProjectWithSubplatform(id);
    }

    public Project ReadProjectFromSubtheme(long id)
    {
        return _projectRepository.ReadProjectFromSubtheme(id);
    }

    public Project ReadProjectFromFlow(long id)
    {
        return _projectRepository.ReadProjectFromFlow(id);
    }

    public Project ReadProjectFromFlowstep(long id)
    {
        return _projectRepository.ReadProjectFromFlowstep(id);
    }

    public IEnumerable<Project> ReadProjectsFromSubplatform(long id)
    {
        return _projectRepository.ReadProjectsFromSubplatform(id);
    }
}