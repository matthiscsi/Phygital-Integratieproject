using PIP.Domain.Deelplatform;
using PIP.Domain.Flow;

namespace PIP.DAL;

public interface IRepository
{
    #region InfoMethods

    Info CreateInfo(Info info);

    #endregion

    #region FlowMethods

    Flow CreateFlow(Flow flow);
    Flow RetrieveFlow(long id);
    IEnumerable<Flow> RetrieveAllFlows();


    IEnumerable<Flow> RetrieveFlowsByProject(long projectId);

    #endregion

    #region FlowStep

    FlowStep CreateFlowStep(FlowStep flowStep);
    FlowStep RetrieveFlowStep(long id);
    IEnumerable<FlowStep> RetrieveAllFlowSteps();

    #endregion

    #region Theme

    Theme CreateTheme(Theme theme);

    Theme RetrieveTheme(long id);


    IEnumerable<Theme> RetrieveAllThemes();

    IEnumerable<Theme> RetrieveAllSubThemesFromMainTheme(long id);

    #endregion


    #region Project

    Project AddProject(Project project);


    Project GetProject(long id);


    IEnumerable<Project> GetAllProjects();


    IEnumerable<Project> GetAllActiveProjects();

    #endregion
}