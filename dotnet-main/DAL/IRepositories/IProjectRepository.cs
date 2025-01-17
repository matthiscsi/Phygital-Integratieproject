using Microsoft.AspNetCore.Identity;
using PIP.Domain.Deelplatform;
using PIP.Domain.User;

namespace PIP.DAL.IRepositories;

public interface IProjectRepository
{
    public Project ReadProject(long id);
    public IEnumerable<Project> ReadActiveProjects(SubPlatformAdministrator user);
    public IEnumerable<Project> ReadProjectsOfUser(IdentityUser user);

    public Project DeleteProject (long id);
    public Project CreateProject(Project project);
    public Project UpdateProject(Project project);
    public Project ReadProjectWithSubplatform(long id);
    public Project ReadProjectFromSubtheme(long id);
    
    public Project ReadProjectFromFlow(long id);
    
    public Project ReadProjectFromFlowstep(long id);
    
    public IEnumerable<Project> ReadProjectsFromSubplatform(long id);
     
    

}