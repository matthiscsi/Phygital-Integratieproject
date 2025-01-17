using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using PIP.BL.IManagers;
using PIP.DAL.IRepositories;
using PIP.Domain.User;

namespace PIP.BL.Managers;

public class UserManager : IUserManager


{
    private readonly IUserRepository _userRepository;

    public UserManager(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public WebappUser AddUser(WebappUser user)
    {

        var validationContext = new ValidationContext(user);
        var validationResults = new List<ValidationResult>();
        bool isValid = Validator.TryValidateObject(user, validationContext, validationResults, true);

        if (!isValid)
        {
        
            foreach (var validationResult in validationResults)
            {
             
                throw new ValidationException(validationResult.ErrorMessage);
            }
        }

        return _userRepository.CreateUser(user);
    }

    public IEnumerable<IdentityUser> GetCompanionsOfProject(int id)
    {
        return _userRepository.ReadCompanionsOfProject(id);
    }

    public Companion AddUserToSubplatform(long subplatform, Companion user)
    {
        return _userRepository.CreateUserToSubplatform(subplatform, user);
    }
    
    public IEnumerable<SubPlatformAdministrator> GetAllSubPlatformAdminsWithSubplatform()
    {
        return _userRepository.ReadAllSubPlatformAdminsWithSubplatform();
    }
    
    public IEnumerable<WebappUser> GetAllUsersFromFlow(int flowId)
    {
        return _userRepository.ReadAllUsersFromFlow(flowId);
    }



    public IEnumerable<WebappUser> DeleteUserByEmail(string email)
    {
        return _userRepository.DeleteUsersByEmail(email);
    }

    public SubPlatformAdministrator GetSubPlatformAdminAndSubplatform(string id)
    {
        return _userRepository.ReadSubPlatformAdminAndSubplatform(id);
    }

    public Companion GetCompanionAndSubplatform(string id)
    {
        return _userRepository.GetCompanionAndSubplatform(id);
    }
}