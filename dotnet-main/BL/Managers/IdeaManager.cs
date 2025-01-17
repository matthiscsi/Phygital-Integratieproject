using System.ComponentModel.DataAnnotations;
using PIP.BL.IManagers;
using PIP.DAL.IRepositories;
using PIP.Domain.WebApplication;

namespace PIP.BL.Managers;

public class IdeaManager : IIdeaManager
{
    private readonly IIdeaRepository _ideaRepository;

    public IdeaManager(IIdeaRepository ideaRepository)
    {
        _ideaRepository = ideaRepository;
    }

    public Idea GetIdeaById(long id)
    {
        return _ideaRepository.ReadIdeaById(id);
    }

    public IEnumerable<Idea> GetAllIdeas()
    {
        return _ideaRepository.ReadAllIdeas();
    }

    public Idea AddIdea(Idea idea)
    {
        return _ideaRepository.CreateIdea(idea);
    }

    public Idea AddIdeaToFlow(Idea idea, long flowId)
    {

        var validationContext = new ValidationContext(idea);
        var validationResults = new List<ValidationResult>();
        bool isValid = Validator.TryValidateObject(idea, validationContext, validationResults, true);

        if (!isValid)
        {
           
            foreach (var validationResult in validationResults)
            {
               
                throw new ValidationException(validationResult.ErrorMessage);
            }
        }

        return _ideaRepository.CreateIdeaToFlow(idea, flowId);
    }
}