using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PIP.BL.IManagers;
using PIP.Domain.User;
using UI.MVC.Models.Dto;

namespace UI.MVC.Controllers.FlowSession.Flow;

public class WebappUserController : Controller
{
    private readonly IUserManager _userManager;
    private readonly IFlowManager _flowManager;


    public WebappUserController(IUserManager userManager, IFlowManager flowManager)
    {
        _userManager = userManager;
        _flowManager = flowManager;
    }


    [HttpPost]
    [Authorize(Roles = "subplatformadministrator")]
    public IActionResult CreateUser(WebappUserDto webappUserDto)
    {
        var flow = _flowManager.GetFlow(webappUserDto.FlowId);
        var user = new WebappUser
        {
            Email = webappUserDto.Email,
            Name = webappUserDto.Name,
            Flow = flow
        };

        List<ValidationResult> errors = new List<ValidationResult>();
        bool isValid =
            Validator.TryValidateObject(user, new ValidationContext(user), errors, validateAllProperties: true);

        if (!isValid)
        {
            StringBuilder sb = new StringBuilder();
            foreach (ValidationResult validationResult in errors)
            {
                sb.Append("|" + validationResult.ErrorMessage);
            }

            TempData["ValidationErrors"] = sb.ToString();

            return RedirectToAction("ThankYou", "Flow", new { id = webappUserDto.FlowId });
        }

        _userManager.AddUser(user);

        TempData["UserCreated"] = true;

        return RedirectToAction("ThankYou", "Flow", new { id = webappUserDto.FlowId });
    }
}