using Microsoft.EntityFrameworkCore;
using PIP.DAL.IRepositories;
using PIP.Domain.WebApplication;

namespace PIP.DAL.EF.Repositories;

public class IdeaRepository : IIdeaRepository
{
    private readonly PhygitalDbContext _context;

    public IdeaRepository(PhygitalDbContext context)
    {
        _context = context;
    }

    public Idea ReadIdeaById(long id)
    {
        return _context.Ideas.Find(id);
    }

    public IEnumerable<Idea> ReadAllIdeas()
    {
        return _context.Ideas;
    }

    public Idea CreateIdea(Idea idea)
    {
        _context.Ideas.Add(idea);
        _context.SaveChanges();
        return idea;
    }

    public Idea CreateIdeaToFlow(Idea idea, long flowId)
    {
        var flow = _context.Flows.Where(f=> f.FlowId == flowId).Include(f=>f.Theme).ThenInclude(t=>t.Ideas).SingleOrDefault();
        if (flow != null)
        {
            if (flow.Theme != null)
            {
                var ideaList = flow.Theme.Ideas.ToList();
                ideaList.Add(idea);
                flow.Theme.Ideas = ideaList;
            }

            _context.SaveChanges();
        }

        return idea;
    }

}