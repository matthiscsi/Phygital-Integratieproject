using PIP.Domain.Deelplatform;
using PIP.Domain.User;

namespace PIP.BL.IManagers;

public interface ISubPlatformManager
{
    IEnumerable<Subplatform> GetAllSubPlatforms(); 
    Subplatform AddSubPlatform(Subplatform subplatform);
    int GetFlowSessionCountOfSubplatform(long subplatformId);
    int GetAverageTimeSpentForFlowSessionsOfSubplatform(long subplatformId);
    Subplatform GetSubplatform(long subplatformId);
    Subplatform GetSubplatformWithProjects(long subplatformId);
    SubPlatformAdministrator GetSubplatformWithAdmin(long subplatformId);

}