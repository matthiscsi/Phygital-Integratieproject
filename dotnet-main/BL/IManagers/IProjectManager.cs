
using Microsoft.AspNetCore.Identity;
using PIP.Domain.Deelplatform;
using PIP.Domain.User;


namespace PIP.BL.IManagers;

public interface IProjectManager
{
    Project GetProject(long id);
    IEnumerable<Project> GetActiveProjects(SubPlatformAdministrator user);
    IEnumerable<Project> GetProjectsOfUser(IdentityUser user);


    Project DeleteProject(long id);
    Project AddProject(Project project);
    Project UpdateProject(Project project);
    int GetFlowSessionCountOfProject(long projectId);
    int GetAverageTimeSpentForFlowsOfProject(long projectId);
    public Project GetProjectWithSubplatform(long id);
    Project ReadProjectFromSubtheme(long id);
    
    Project ReadProjectFromFlow(long id);
    
    Project ReadProjectFromFlowstep(long id);
    
    IEnumerable<Project> ReadProjectsFromSubplatform(long id);
}