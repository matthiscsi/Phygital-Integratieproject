using PIP.Domain.WebApplication;

namespace PIP.BL.IManagers;

public interface IIdeaManager
{
    Idea GetIdeaById(long id);
    IEnumerable<Idea> GetAllIdeas();
    Idea AddIdea(Idea idea);
    Idea AddIdeaToFlow(Idea idea, long flowId);
}