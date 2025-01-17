using Microsoft.AspNetCore.Identity;
using PIP.Domain.User;

namespace PIP.DAL.IRepositories;

public interface IUserRepository
{
    
      WebappUser CreateUser(WebappUser user);
      
      IEnumerable<IdentityUser> ReadCompanionsOfProject(int id);
      Companion CreateUserToSubplatform(long subplatform, Companion user);
      IEnumerable<SubPlatformAdministrator> ReadAllSubPlatformAdminsWithSubplatform();
      
      IEnumerable<WebappUser> ReadAllUsersFromFlow(int flowId);
      SubPlatformAdministrator ReadSubPlatformAdminAndSubplatform(string id);
 

      IEnumerable<WebappUser> DeleteUsersByEmail(string email);

      Companion GetCompanionAndSubplatform(string id);
}