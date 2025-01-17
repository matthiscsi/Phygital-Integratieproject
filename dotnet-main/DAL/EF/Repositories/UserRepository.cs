using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PIP.DAL.IRepositories;
using PIP.Domain.User;

namespace PIP.DAL.EF.Repositories;

public class UserRepository : IUserRepository


{
    private readonly PhygitalDbContext _context;

    public UserRepository(PhygitalDbContext context)
    {
        _context = context;
    }

    public WebappUser CreateUser(WebappUser user)
    {
        _context.WebappUsers.Add(user);
        _context.SaveChanges();
        return user;
    }

    public IEnumerable<IdentityUser> ReadCompanionsOfProject(int id)
    {
        return _context.Projects.Find(id)?.Companions.ToList();
    }

    public Companion CreateUserToSubplatform(long subplatformId, Companion user)
    {
        // Find the subplatform
        var subplatform = _context.Subplatforms.Find(subplatformId);

        // Check if the subplatform exists
        if (subplatform != null)
        {
            // Assign the subplatform to the user
            user.Subplatform = subplatform;

            // Save the changes
            _context.SaveChanges();
        }

        return user;
    }
    public IEnumerable<SubPlatformAdministrator> ReadAllSubPlatformAdminsWithSubplatform()
    {
        var roleId = _context.Roles
            .Where(r => r.Name == "subplatformadministrator")
            .Select(r => r.Id)
            .FirstOrDefault();

        var userIds = _context.UserRoles
            .Where(ur => ur.RoleId == roleId)
            .Select(ur => ur.UserId)
            .ToList();

        var users = _context.Deelplatformadministrators
            .Include(deelplatformadministrator => deelplatformadministrator.Subplatform)
            .Where(u => userIds.Contains(u.Id)).AsEnumerable()
            .Select(u => new SubPlatformAdministrator()
            {
                Id = u.Id,
                UserName = u.UserName,
                Email = u.Email,
                Subplatform = u.Subplatform,
                OrganizationName = u.OrganizationName
            });

        return users;
    }

    public IEnumerable<WebappUser> ReadAllUsersFromFlow(int flowId)
    {
        return _context.WebappUsers.Where(w => w.Flow.FlowId == flowId);
    }

    public SubPlatformAdministrator ReadSubPlatformAdminAndSubplatform(string id)
    {
        return _context.Deelplatformadministrators
            .Include(deelplatformadministrator => deelplatformadministrator.Subplatform)
            .FirstOrDefault(u => u.Id == id);
    }

    public IEnumerable<WebappUser> DeleteUsersByEmail(string email)
    {
        var users = _context.WebappUsers.Where(u => u.Email == email);
    
        if (users.Any())
        {
            _context.WebappUsers.RemoveRange(users);
            _context.SaveChanges();
        }
    
        return users;
    }

    public Companion GetCompanionAndSubplatform(string id)
    {
        return _context.Companions
            .Include(companion => companion.Subplatform)
            .FirstOrDefault(c => c.Id == id);
    }
} 