using System.Collections;
using System.ComponentModel.DataAnnotations;
using IP.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PIP.BL.IManagers;
using PIP.Domain.Flow;
using PIP.Domain.Flow.Inquiry;
using UI.MVC.Models;
using UI.MVC.Models.Dto;
using UI.MVC.Models.Dto.AnswerDto;

namespace UI.MVC.Controllers.api;

[ApiController]
[Route("/api/[controller]")]
public class FlowstepsController : ControllerBase
{
    private readonly IFlowManager _flowManager;
    private readonly IFlowSessionManager _flowSessionManager;
    private readonly IFlowStepManager _flowStepManager;
    private readonly ISubthemeManager _subthemeManager;
    private readonly IProjectManager _projectManager;
    private readonly FlowStepHelper _flowstepHelper;


    public FlowstepsController(IFlowManager flowManager, IFlowSessionManager flowSessionManager,
        IFlowStepManager flowStepManager, ISubthemeManager subthemeManager, IProjectManager projectManager,
        FlowStepHelper flowStepHelper)
    {
        _projectManager = projectManager;
        _flowManager = flowManager;
        _flowSessionManager = flowSessionManager;
        _flowStepManager = flowStepManager;
        _subthemeManager = subthemeManager;
        _flowstepHelper = flowStepHelper;
    }

    [HttpGet("getStep/{id:long}")]
    
    public IActionResult GetFlowStep(long id)
    {
        var currentFlowStep = _flowStepManager.GetFlowStep(id);
        if (currentFlowStep == null)
        {
            return NotFound("Current step not found or session is invalid.");
        }

        var responseData = _flowstepHelper.PassDataInDto(currentFlowStep);

        if (responseData == null)
        {
            return NotFound();
        }
        return Ok(responseData);
    }

    [HttpGet("current/{id:long}")]
    public IActionResult GetCurrentStep(long id)
    {
        var currentFlowStep = _flowSessionManager.GetCurrentStep(id);
        if (currentFlowStep == null)
        {
            return NotFound("Current step not found or session is invalid.");
        }

        
        var responseData = _flowstepHelper.PassDataInDto(currentFlowStep);
        if (responseData == null)
        {
            return NotFound();
        }
        return Ok(responseData);
    }


    [HttpGet("next/{id}")]
    public IActionResult MoveToNextStep(long id)
    {
        var nextStep = _flowSessionManager.MoveToNextStep(id);
        var flowSession = _flowSessionManager.GetFlowSession(id);
        var flow = flowSession.CirculaireFlows;

        if (flow == null)
        {
            if (nextStep == null)
            {
                return NotFound();
            }
        }
        else
        {
            if (nextStep == null)
            {
                _flowManager.UpdateFlowSessionCirculaireFlow(id);
                nextStep = _flowSessionManager.GetCurrentStep(id);
            }
        }

        var responseData = _flowstepHelper.PassDataInDto(nextStep);
        if (responseData == null)
        {
            return NotFound();
        }
        return Ok(responseData);
    }


    [HttpGet("skipsubtheme/{id:long}")]
    public IActionResult SkipSubTheme(long id)
    {
        var flowSession = _flowSessionManager.GetFlowSession(id);
        if (flowSession == null || flowSession.CurrentFlowStep == null)
        {
            return NotFound();
        }

        var initialSubtheme = flowSession.CurrentFlowStep.SubTheme;
        if (initialSubtheme == null)
        {
            return NotFound();
        }

        _flowSessionManager.SkipSubtheme(initialSubtheme.SubthemeId, flowSession.FlowSessionId);
        return Ok();
    }

    [HttpGet("{id:long}")]
    public IActionResult GetFlowstepsOfFlow(long id)
    {
        var flow = _flowStepManager.GetFlowStepsByFlowId(id);
        var flowsteps = new ArrayList();
        foreach (FlowStep flowStep in flow.FlowSteps)
        {
            flowsteps.Add(_flowstepHelper.PassDataInDto(flowStep));
        }

        return Ok(flowsteps);
    }


    [HttpPost]
    [Authorize(Roles = "subplatformadministrator")]
    [Route("post")]
    public Task<IActionResult> AddFlowStep([FromBody] FlowStepDtoWassim flowStep)
    {
        if (!User.Identity!.IsAuthenticated)
        {
            return Task.FromResult<IActionResult>(Redirect("/Identity/Account/Login"));
        }

        var flow = _flowManager.GetFlow(flowStep.Flow);
        var project = _projectManager.GetProject(flow.Project.ProjectId);
        bool userManager = User.Identity!.Name == project.SubPlatformAdmin.UserName;
        if (!(User.IsInRole("subplatformadministrator") || userManager))
        {
            return Task.FromResult<IActionResult>(Unauthorized());
        }
        
        if (!ModelState.IsValid)
        {
            return Task.FromResult<IActionResult>(BadRequest(ModelState));
        }

        try
        {
            var subtheme = _subthemeManager.GetSubtheme(flowStep.SubthemeId);
            switch (flowStep.FlowStepType)
            {
                case "Info":
                    var info = new Info
                    {
                        Body = flowStep.Body,
                        OrderNr = flowStep.Ordernr,
                        UrlImage = flowStep.UploadedImage,
                        UrlVideo = flowStep.UploadedVideo,
                        UrlAudio = flowStep.UploadedAudio,
                        Flow = flow,
                        SubTheme = subtheme,
                        IsActive = true
                    };
                    
                    var validationContext = new ValidationContext(info);
                    var validationResults = new List<ValidationResult>();
                    bool isValid = Validator.TryValidateObject(info, validationContext, validationResults, true);

                    if (!isValid)
                    {
                        foreach (var validationResult in validationResults)
                        {
                            ModelState.AddModelError(validationResult.MemberNames.First(), validationResult.ErrorMessage);
                        }
                        return Task.FromResult<IActionResult>(BadRequest(ModelState));
                    }

                    _flowStepManager.AddInfo(info);
                    break;
                case "MultipleChoiceQuestion":
                    var multipleChoiceQuestion = new MultipleChoiceQuestion
                    {
                        Query = flowStep.Query,
                        OrderNr = flowStep.Ordernr,
                        Options = flowStep.Options,
                        Flow = flow,
                        SubTheme = subtheme,
                        IsActive = true
                    };
                    validationContext = new ValidationContext(multipleChoiceQuestion);
                    validationResults = new List<ValidationResult>();
                    isValid = Validator.TryValidateObject(multipleChoiceQuestion, validationContext, validationResults, true);

                    if (!isValid)
                    {
                        foreach (var validationResult in validationResults)
                        {
                            ModelState.AddModelError(validationResult.MemberNames.First(), validationResult.ErrorMessage);
                        }
                        return Task.FromResult<IActionResult>(BadRequest(ModelState));
                    }
                    _flowStepManager.AddMultipleChoiceQuestion(multipleChoiceQuestion);

                    break;
                case "ClosedQuestion":
                    var closedQuestion = new ClosedQuestion
                    {
                        Query = flowStep.Query,
                        OrderNr = flowStep.Ordernr,
                        Options = flowStep.Options,
                        Flow = flow,
                        SubTheme = subtheme,
                        IsActive = true
                    };
                    validationContext = new ValidationContext(closedQuestion);
                    validationResults = new List<ValidationResult>();
                    isValid = Validator.TryValidateObject(closedQuestion, validationContext, validationResults, true);

                    if (!isValid)
                    {
                        foreach (var validationResult in validationResults)
                        {
                            ModelState.AddModelError(validationResult.MemberNames.First(), validationResult.ErrorMessage);
                        }
                        return Task.FromResult<IActionResult>(BadRequest(ModelState));
                    }
                    _flowStepManager.AddClosedQuestion(closedQuestion);

                    break;
                case "RangeQuestion":
                    var rangeQuestion = new RangeQuestion
                    {
                        Query = flowStep.Query,
                        OrderNr = flowStep.Ordernr,
                        MinValue = flowStep.MinValue,
                        MaxValue = flowStep.MaxValue,
                        Flow = flow,
                        SubTheme = subtheme,
                        IsActive = true
                    };
                    validationContext = new ValidationContext(rangeQuestion);
                    validationResults = new List<ValidationResult>();
                    isValid = Validator.TryValidateObject(rangeQuestion, validationContext, validationResults, true);

                    if (!isValid)
                    {
                        foreach (var validationResult in validationResults)
                        {
                            ModelState.AddModelError(validationResult.MemberNames.First(), validationResult.ErrorMessage);
                        }
                        return Task.FromResult<IActionResult>(BadRequest(ModelState));
                    }
                    _flowStepManager.AddRangeQuestion(rangeQuestion);


                    break;
                case "OpenQuestion":
                    var openQuestion = new OpenQuestion
                    {
                        Query = flowStep.Query,
                        OrderNr = flowStep.Ordernr,
                        Flow = flow,
                        SubTheme = subtheme,
                        IsActive = true
                    };
                    validationContext = new ValidationContext(openQuestion);
                    validationResults = new List<ValidationResult>();
                    isValid = Validator.TryValidateObject(openQuestion, validationContext, validationResults, true);

                    if (!isValid)
                    {
                        foreach (var validationResult in validationResults)
                        {
                            ModelState.AddModelError(validationResult.MemberNames.First(), validationResult.ErrorMessage);
                        }
                        return Task.FromResult<IActionResult>(BadRequest(ModelState));
                    }
                    _flowStepManager.AddOpenQuestion(openQuestion);


                    break;
                default:

                    return Task.FromResult<IActionResult>(BadRequest("Unknown flow step type"));
            }


            return Task.FromResult<IActionResult>(Ok("Flow step added successfully"));
        }
        catch (Exception ex)
        {
            return Task.FromResult<IActionResult>(StatusCode(500, $"An error occurred: {ex.Message}"));
        }
    }

    [HttpDelete]
    [Route("delete/{id}")]
    [Authorize(Roles = "subplatformadministrator")]
    public IActionResult DeleteFlowstep(long id)
    {
        var flowstep = _flowStepManager.GetFlowStep(id);

        if (!User.Identity!.IsAuthenticated)
        {
            return Redirect("/Identity/Account/Login");
        }

        var flow = _flowManager.GetFlow(flowstep.Flow.FlowId);
        var project = _projectManager.GetProject(flow.Project.ProjectId);
        bool userManager = User.Identity.Name == project.SubPlatformAdmin.UserName;
        if (!userManager)
        {
            return Unauthorized();
        }

        switch (flowstep)
        {
            case ClosedQuestion closedQuestion:
                _flowStepManager.DeleteClosedQuestion(closedQuestion);
                break;
            case MultipleChoiceQuestion multipleChoiceQuestion:
                _flowStepManager.DeleteMultipleChoiceQuestion(multipleChoiceQuestion);
                break;
            case RangeQuestion rangeQuestion:
                _flowStepManager.DeleteRangeQuestion(rangeQuestion);
                break;
            case OpenQuestion openQuestion:
                _flowStepManager.DeleteOpenQuestion(openQuestion);
                break;
            case Info info:
                _flowStepManager.DeleteInfo(info);
                break;
            default:
                return BadRequest("Unknown flow step type");
        }


        return Ok("Flowstep deleted successfully");
    }

    [HttpPut("{id:long}/order/{newOrder:int}")]
    [Authorize(Roles = "subplatformadministrator")]
    public IActionResult UpdateFlowStepOrder(long id, int newOrder)
    {
        var flowStep = _flowStepManager.GetFlowStep(id);

        if (!User.Identity!.IsAuthenticated)
        {
            return Redirect("/Identity/Account/Login");
        }

        var flow = _flowManager.GetFlow(flowStep.Flow.FlowId);
        var project = _projectManager.GetProject(flow.Project.ProjectId);
        bool userManager = User.Identity.Name == project.SubPlatformAdmin.UserName;
        if (!userManager)
        {
            return Unauthorized();
        }

        if (flowStep == null)
        {
            return NotFound("Flow step not found");
        }
        flowStep.OrderNr = newOrder;

        flowStep = _flowStepManager.UpdateFlowStep(flowStep);
        if (flowStep == null)
        {
            return NotFound();
        }

        return Ok(flowStep);
    }

    [HttpPut("updateflowstep/{id:long}")]
    public IActionResult UpdateFlowStep(long id, [FromBody] FlowStepDtoWassim model)
    {
        var flowStep = _flowStepManager.GetFlowStep(id);
        if (flowStep == null)
        {
            return NotFound();
        }

        flowStep.IsActive = model.IsActive;

        switch (flowStep)
        {
            case ClosedQuestion closedQuestion:
                closedQuestion.Query = model.Query;
                closedQuestion.IsActive = model.IsActive;
                _flowStepManager.UpdateClosedQuestion(closedQuestion);

                break;
            case MultipleChoiceQuestion multipleChoiceQuestion:
                multipleChoiceQuestion.Query = model.Query;
                multipleChoiceQuestion.IsActive = model.IsActive;
                _flowStepManager.UpdateMultipleChoiceQuestion(multipleChoiceQuestion);
                break;
            case RangeQuestion rangeQuestion:
                rangeQuestion.MinValue = model.MinValue;
                rangeQuestion.MaxValue = model.MaxValue;
                rangeQuestion.Query = model.Query;
                rangeQuestion.IsActive = model.IsActive;
                _flowStepManager.UpdateRangeQuestion(rangeQuestion);
                break;
            case OpenQuestion openQuestion:
                openQuestion.Query = model.Query;
                openQuestion.IsActive = model.IsActive;
                _flowStepManager.UpdateOpenQuestion(openQuestion);
                break;
            default:
                return BadRequest("Unsupported flow step type");
        }

        Subtheme subtheme = _subthemeManager.GetSubtheme(model.SubthemeId);
        flowStep.SubTheme = subtheme;
        _flowStepManager.UpdateFlowStep(flowStep);

        return Ok(flowStep);
    }

    // [Authorize(Roles = "subplatformadministrator")]
    [HttpPost]
    [Route("postConditionalPoint")]
    public IActionResult AddConditionalPoint([FromBody] ConditionalPointDto conditionalPointDto)
    {
        FlowStepDtoWassim flowStepDto = conditionalPointDto.FollowUpStep;
        FlowStep flowStep = null;
        switch (flowStepDto.FlowStepType)
        {
            case "Info":
                var info = new Info
                {
                    Body = flowStepDto.Body,
                    UrlImage = flowStepDto.UploadedImage,
                    UrlVideo = flowStepDto.UploadedVideo,
                    UrlAudio = flowStepDto.UploadedAudio
                };

                flowStep = info;
                break;
            case "MultipleChoiceQuestion":
                var multipleChoiceQuestion = new MultipleChoiceQuestion
                {
                    Query = flowStepDto.Query,
                    Options = flowStepDto.Options
                };
                flowStep = multipleChoiceQuestion;


                break;
            case "ClosedQuestion":
                var closedQuestion = new ClosedQuestion
                {
                    Query = flowStepDto.Query,
                    Options = flowStepDto.Options
                };
                flowStep = closedQuestion;


                break;
            case "RangeQuestion":
                var rangeQuestion = new RangeQuestion
                {
                    Query = flowStepDto.Query,
                    MinValue = flowStepDto.MinValue,
                    MaxValue = flowStepDto.MaxValue
                };
                flowStep = rangeQuestion;


                break;
            case "OpenQuestion":
                var openQuestion = new OpenQuestion
                {
                    Query = flowStepDto.Query,
                    IsActive = true
                };
                flowStep = openQuestion;
                break;
        }


        Answer answer = IdentifyAnswer(conditionalPointDto.Criteria);
        if (flowStep==null)
        {
            return BadRequest();
        }
        _flowStepManager.AddConditionalPoint(conditionalPointDto.Question.FlowStepId, flowStep, answer);
        return Ok();
    }

    public Answer IdentifyAnswer(AnswerDto answerDto)
    {
        Answer answer = null;
        switch (answerDto.AnswerType)
        {
            case "MultipleChoice":
                answer = new MultipleChoiceAnswer()
                {
                    SelectedAnswers = answerDto.SelectedAnswers
                };
                break;
            case "Closed":
                answer = new ClosedAnswer()
                {
                    SelectedAnswer = answerDto.SelectedAnswer
                };
                break;
            case "Open":
                answer = new OpenAnswer()
                {
                    Answer = answerDto.answer
                };
                break;
            case "Range":
                if (answerDto.SelectedAnswer != null)
                    answer = new RangeAnswer()
                    {
                        SelectedValue = answerDto.SelectedAnswer
                    };
                break;
        }

        return answer;
    }

    [HttpPost]
    [Route("removeConditionalPoint/{conditionalPointId:long}")]
    public IActionResult RemoveConditionalPoint(long conditionalPointId)
    {
        _flowStepManager.RemoveConditionalPoint(conditionalPointId);
        return Ok();
    }
    
}