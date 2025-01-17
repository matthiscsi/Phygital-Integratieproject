using PIP.Domain.Deelplatform;
using PIP.Domain.User;

namespace PIP.DAL.IRepositories;

public interface ISubPlatformRepository
{
    
    IEnumerable<Subplatform> ReadAllSubPlatforms();
    Subplatform CreateSubPlatform(Subplatform subplatform);
    Subplatform ReadSubplatformWithProjects(long subplatformId);
    Subplatform ReadSubplatform(long subplatformId);
    SubPlatformAdministrator  GetSubplatformAdmin(long subplatformId);
}