using PIP.Domain.WebApplication;

namespace PIP.DAL.IRepositories;

public interface IIdeaRepository
{
    Idea ReadIdeaById(long id);
    IEnumerable<Idea> ReadAllIdeas();
    Idea CreateIdea(Idea idea);
    Idea CreateIdeaToFlow(Idea idea, long flowId);
}