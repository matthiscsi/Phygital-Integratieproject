using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PIP.BL.IManagers;
using PIP.Domain.Flow.Inquiry;
using UI.MVC.Models.Dto;

namespace UI.MVC.Controllers.api;

[ApiController]
[Route("/api/[controller]")]
public class OptionsController : ControllerBase
{
    private readonly IFlowStepManager _flowStepManager;
    private readonly IProjectManager _projectManager;


    public OptionsController(IFlowStepManager flowStepManager, IProjectManager projectManager)
    {
        _flowStepManager = flowStepManager;
        _projectManager = projectManager;
    }

    [HttpPost]
    [Route("addoption")]
    [Authorize(Roles = "subplatformadministrator")]
    public IActionResult AddOption(Option option, long questionId)
    {
        if (!User.Identity!.IsAuthenticated)
        {
            return Redirect("/Identity/Account/Login");
        }

        var project = _projectManager.ReadProjectFromFlowstep(questionId);
        bool userManager = User.Identity.Name == project.SubPlatformAdmin.UserName;
        if (!userManager)
        {
            return Unauthorized();
        }

        var createdOption = _flowStepManager.AddOptionToQuestion(option, questionId);

        if (createdOption == null)
        {
            return BadRequest("Failed to add option to question");
        }

        return CreatedAtAction(nameof(AddOption), new { id = createdOption.Id }, createdOption);
    }

    [HttpDelete]
    [Route("deleteoption")]
    [Authorize(Roles = "subplatformadministrator")]
    public IActionResult DeleteOption(long optionId, long questionId)
    {
        if (!User.Identity!.IsAuthenticated)
        {
            return Redirect("/Identity/Account/Login");
        }

        var project = _projectManager.ReadProjectFromFlowstep(questionId);
        bool userManager = User.Identity.Name == project.SubPlatformAdmin.UserName;
        if (!userManager)
        {
            return Unauthorized();
        }

        var option = _flowStepManager.RetrieveOption(optionId);
        if (option == null)
        {
            return NotFound("Option not found");
        }

        _flowStepManager.DeleteOptionFromQuestion(option, questionId);

        return Ok();
    }

    [HttpPut("updateoptions/{id:long}")]
    [Authorize(Roles = "subplatformadministrator")]
    public IActionResult UpdateOptions(IEnumerable<OptionDto> options, long id)
    {
        if (!User.Identity!.IsAuthenticated)
        {
            return Redirect("/Identity/Account/Login");
        }

        var project = _projectManager.ReadProjectFromFlowstep(id);
        bool userManager = User.Identity.Name == project.SubPlatformAdmin.UserName;
        if (!userManager)
        {
            return Unauthorized();
        }

        Question question = _flowStepManager.GetQuestion(id);


        if (question == null)
        {
            return NotFound("Question not found");
        }


        if (question is MultipleChoiceQuestion)
        {
            question = _flowStepManager.GetMultipleChoiceQuestionWithOptions(id);
        }
        else if (question is ClosedQuestion)
        {
            question = _flowStepManager.GetClosedQuestionWithOptions(id);
        }
        else
        {
            return BadRequest("Question type does not support options");
        }

        var existingOptions = _flowStepManager.GetOptionsByQuestionId(id);


        var enumerable = existingOptions.ToList();
        var optionDtos = options.ToList();
        foreach (var existingOption in enumerable)
        {
            if (optionDtos.All(o => o.Id != existingOption.Id))
            {
                _flowStepManager.DeleteOptionFromQuestion(existingOption, id);
            }
        }

        foreach (var optionDto in optionDtos)
        {
            if (!enumerable.Any(o => o.Id == optionDto.Id))
            {
                if (optionDto.Id == -1 && enumerable.Any(o => o.Text == optionDto.Text) &&
                    enumerable.Any(o => o.Question.FlowStepId == question.FlowStepId))
                {
                    break;
                }

                if (!enumerable.Any(o => o.Text == optionDto.Text))
                {
                    var newOption = new Option
                    {
                        Text = optionDto.Text,
                        Question = question
                    };
                    _flowStepManager.AddOptionToQuestion(newOption, id);
                }
            }
        }


        foreach (var optionDto in optionDtos)
        {
            var existingOption = enumerable.FirstOrDefault(o => o.Id == optionDto.Id);
            if (existingOption != null)
            {
                existingOption.Text = optionDto.Text;
                _flowStepManager.UpdateOption(existingOption);
            }
        }

        return Ok(_flowStepManager.GetOptionsByQuestionId(id));
    }

    [HttpGet]
    [Route("getoptions")]
    [Authorize(Roles = "subplatformadministrator")]
    public IActionResult GetOptions(long questionId)
    {
        if (!User.Identity!.IsAuthenticated)
        {
            return Redirect("/Identity/Account/Login");
        }

        var project = _projectManager.ReadProjectFromFlowstep(questionId);
        bool userManager = User.Identity.Name == project.SubPlatformAdmin.UserName;
        if (!userManager)
        {
            return Unauthorized();
        }

        var options = _flowStepManager.GetOptionsByQuestionId(questionId);

        if (options == null)
        {
            return NotFound("No options found for the given question");
        }

        return Ok(options);
    }
}