using PIP.BL.IManagers;
using PIP.DAL.IRepositories;
using PIP.Domain.Deelplatform;
using PIP.Domain.User;

namespace PIP.BL.Managers;

public class SubplatformManager : ISubPlatformManager
{
    private readonly ISubPlatformRepository _subPlatformRepository;
    private readonly IProjectManager _projectManager;

    public SubplatformManager(ISubPlatformRepository subPlatformRepository, IProjectManager projectManager)
    {
        _subPlatformRepository = subPlatformRepository;
        _projectManager = projectManager;
    }

    public IEnumerable<Subplatform> GetAllSubPlatforms()
    {
        return _subPlatformRepository.ReadAllSubPlatforms();
    }
     
    public Subplatform AddSubPlatform(Subplatform subplatform)
    {
        return _subPlatformRepository.CreateSubPlatform(subplatform);
    }

    public int GetFlowSessionCountOfSubplatform(long subplatformId)
    {
        int flowSessionCount = 0;
        var subplatform = _subPlatformRepository.ReadSubplatformWithProjects(subplatformId);
        foreach (var project in subplatform.Projects)
        {
            flowSessionCount += _projectManager.GetFlowSessionCountOfProject(project.ProjectId);
        }

        return flowSessionCount;
    }

    public int GetAverageTimeSpentForFlowSessionsOfSubplatform(long subplatformId)
    {
        int averageTime = 0;
        var subplatform = _subPlatformRepository.ReadSubplatformWithProjects(subplatformId);
        foreach (var project in subplatform.Projects)
        {
            averageTime += _projectManager.GetAverageTimeSpentForFlowsOfProject(project.ProjectId);
        }

        return averageTime;
    }

    public Subplatform GetSubplatform(long subplatformId)
    {
        return _subPlatformRepository.ReadSubplatform(subplatformId);
    }
    public Subplatform GetSubplatformWithProjects(long subplatformId)
    {
        return _subPlatformRepository.ReadSubplatformWithProjects(subplatformId);
    }

    public SubPlatformAdministrator GetSubplatformWithAdmin(long subplatformId)
    {
        return _subPlatformRepository.GetSubplatformAdmin(subplatformId);
    }
}