using System.ComponentModel.DataAnnotations;
using PIP.BL.IManagers;
using PIP.DAL.IRepositories;
using PIP.Domain.Flow;

namespace PIP.BL.Managers;

public class SubthemeManager: ISubthemeManager
{
    
    private readonly ISubthemeRepository _subthemeRepository;
    
    public SubthemeManager(ISubthemeRepository subthemeRepository)
    {
        _subthemeRepository = subthemeRepository;
    }


    public Subtheme AddSubtheme(Subtheme subtheme)
    {
        return _subthemeRepository.CreateSubtheme(subtheme);
    }
     
    public Subtheme GetSubtheme(long id)
    {
        return _subthemeRepository.ReadSubtheme(id);
    }

    public Subtheme DeleteSubtheme(long id)
    {
        return _subthemeRepository.DeleteSubtheme(id);
    }
    
    public Subtheme UpdateSubtheme(Subtheme subtheme)
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(subtheme);
        if (!Validator.TryValidateObject(subtheme, validationContext, validationResults, true))
        {
            var errors = validationResults.Select(vr => vr.ErrorMessage);
            throw new ValidationException("Subtheme validation failed: " + string.Join(", ", errors));
        }

        return _subthemeRepository.UpdateSubtheme(subtheme);
    }

  
}