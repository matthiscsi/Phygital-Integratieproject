using PIP.DAL.IRepositories;

namespace PIP.DAL.EF.Repositories;

public class FlowsessionRepository : IFlowsessionRepository
{ 
    private readonly PhygitalDbContext _context;

    public FlowsessionRepository(PhygitalDbContext context)
    {
        _context = context;
    }
    
    public int GetTotalFlowSessionCount()
    {
        return _context.FlowSessions.Count();
    }
}