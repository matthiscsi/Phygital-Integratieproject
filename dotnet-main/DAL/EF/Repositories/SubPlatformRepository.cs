using Microsoft.EntityFrameworkCore;
using PIP.DAL.IRepositories;
using PIP.Domain.Deelplatform;
using PIP.Domain.User;

namespace PIP.DAL.EF.Repositories;

public class SubPlatformRepository : ISubPlatformRepository
{
    
    
      private readonly PhygitalDbContext _context;
      
        public SubPlatformRepository(PhygitalDbContext dbContext)
        {
            _context = dbContext;
        }
    public IEnumerable<Subplatform> ReadAllSubPlatforms()
    {
        return _context.Subplatforms;
    }

    public Subplatform CreateSubPlatform(Subplatform subplatform)
    {
        _context.Subplatforms.Add(subplatform);
        _context.SaveChanges();

        return subplatform;
    }

    public Subplatform ReadSubplatformWithProjects(long subplatformId)
    {
        return _context.Subplatforms.Include(s=>s.Projects)
            .Single(s => s.SubplatformId == subplatformId);
    }

    public Subplatform ReadSubplatform(long subplatformId)
    {
        return _context.Subplatforms.Single(s => s.SubplatformId == subplatformId);
    }

    public SubPlatformAdministrator GetSubplatformAdmin(long subplatformId)
    {
        return _context.Deelplatformadministrators
            .Include(d => d.Subplatform)
            .OfType<SubPlatformAdministrator>()
            .SingleOrDefault(d => d.Subplatform.SubplatformId == subplatformId);
    }


}