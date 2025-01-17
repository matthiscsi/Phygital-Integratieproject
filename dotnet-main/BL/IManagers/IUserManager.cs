using Microsoft.AspNetCore.Identity;
using PIP.Domain.User;

namespace PIP.BL.IManagers;

public interface IUserManager
{
    public WebappUser AddUser(WebappUser user);
    public IEnumerable<IdentityUser> GetCompanionsOfProject(int id);
    public Companion AddUserToSubplatform(long subplatform, Companion user);
    public IEnumerable<SubPlatformAdministrator> GetAllSubPlatformAdminsWithSubplatform();

    public IEnumerable<WebappUser> GetAllUsersFromFlow(int flowId);


    IEnumerable<WebappUser> DeleteUserByEmail(string email);


    SubPlatformAdministrator GetSubPlatformAdminAndSubplatform(string id);
    
    Companion GetCompanionAndSubplatform(string id);
}