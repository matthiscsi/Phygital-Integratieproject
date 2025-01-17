using Humanizer;
using IP.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PIP.BL;
using PIP.BL.IManagers;
using PIP.Domain.Deelplatform;
using PIP.Domain.Flow;
using UI.MVC.Models;
using PIP.Domain.Flow.Inquiry;
using UI.MVC.Models.Dto;
using UI.MVC.Models.Dto.AnswerDto;

namespace UI.MVC.Controllers.FlowSession.Flow;

public class FlowStepController : Controller
{
    private readonly IFlowSessionManager _flowSessionManager;

    private readonly IFlowStepManager _flowStepManager;
    private readonly ISubthemeManager _subthemeManager;
    private readonly IFlowManager _flowManager;
    private readonly ICloudBucketManager _cloudBucketManager;
    private readonly IProjectManager _projectManager;


    public FlowStepController(IFlowSessionManager flowSessionManager, IFlowStepManager flowStepManager,
        ISubthemeManager subthemeManager, IFlowManager flowManager, ICloudBucketManager cloudBucketManager,
        IProjectManager projectManager)
    {
        _flowSessionManager = flowSessionManager;
        _flowStepManager = flowStepManager;
        _subthemeManager = subthemeManager;
        _flowManager = flowManager;
        _cloudBucketManager = cloudBucketManager;
        _projectManager = projectManager;
    }
    public IActionResult CompanionFlowStepIndex(long id)
    {
        
        var flowSession = _flowSessionManager.GetFlowSession(id);
        if (flowSession == null)
        {
            TempData["RedirectMessage"] = "De flowsession is niet gevonden.";
            return RedirectToAction("Startup", "Home" , new { id = 1 });
        }
        Project project;
        if (flowSession.CirculaireFlows != null)
        {
            project = _projectManager.GetProject(flowSession.CirculaireFlows.Flows.First().Project.ProjectId);
            ViewData["Project"] = project;
            return View(flowSession);
        }
        project = _projectManager.GetProject(flowSession.Flow.Project.ProjectId);
        ViewData["Project"] = project;
        return View(flowSession);
    }
    public IActionResult Index(long id)
    {
        var flowSession = _flowSessionManager.GetFlowSession(id);
        if (flowSession == null)
        {
            TempData["RedirectMessage"] = "De flowsession is niet gevonden.";
            return RedirectToAction("Startup", "Home" , new { id = 1 });
        }
            
        Project project;
        if (flowSession.CirculaireFlows != null)
        {
            project = _projectManager.GetProject(flowSession.CirculaireFlows.Flows.First().Project.ProjectId);
            ViewData["Project"] = project;
            return View(flowSession);
        }
        project = _projectManager.GetProject(flowSession.Flow.Project.ProjectId);
        _cloudBucketManager.GenerateQrCode("flowSessionBegeleider", flowSession.FlowSessionId);
        ViewData["Project"] = project;
        return View(flowSession);
    }
    public IActionResult WaitingRoom(long id)
    {
        var flowSession = _flowSessionManager.GetFlowSession(id);
        if (flowSession == null)
        {
            TempData["RedirectMessage"] = "De flowsession is niet gevonden.";
            return RedirectToAction("Startup", "Home" , new { id = 1 });
        }
        var project = _projectManager.GetProject(flowSession.Flow.Project.ProjectId);
        // Generate the QR codes
        _cloudBucketManager.GenerateQrCode("flowSessionClient", id);
        _cloudBucketManager.GenerateQrCode("flowSessionHost", id);

        // Store the URLs of the QR codes in the ViewBag
        ViewBag.ParticipantQrCodeUrl =
            $"https://storage.googleapis.com/phygitalmediabucket/qr-code-flowSessionClient-{id}.png";
        ViewBag.GuideQrCodeUrl = $"https://storage.googleapis.com/phygitalmediabucket/qr-code-flowSessionHost-{id}.png";

        ViewData["Project"] = project;
        return View(flowSession);
    }
    
    public IActionResult HostFlowStepIndex(long id)
    {
        var flowSession = _flowSessionManager.GetFlowSession(id);
        if (flowSession == null)
        {
            TempData["RedirectMessage"] = "De flowsession is niet gevonden.";
            return RedirectToAction("Startup", "Home" , new { id = 1 });
        }
        var project = _projectManager.GetProject(flowSession.Flow.Project.ProjectId);
        ViewData["Project"] = project;
        return View(flowSession);
    }
    public IActionResult ClientFlowStepIndex(long id)
    {
        var flowSession = _flowSessionManager.GetFlowSession(id);
        if (flowSession == null)
        {
            TempData["RedirectMessage"] = "De flowsession is niet gevonden.";
            return RedirectToAction("Startup", "Home" , new { id = 1 });
        }
        var project = _projectManager.GetProject(flowSession.Flow.Project.ProjectId);
        ViewData["Project"] = project;
        return View(flowSession);
    }

    [Authorize(Roles = "subplatformadministrator")]
    public IActionResult Edit(long id)
    {
        var flowStep = _flowStepManager.GetFlowStep(id);
        if ( flowStep == null)
        {
            TempData["RedirectMessage"] = "De flowstep is niet gevonden.";
            return RedirectToAction("Startup", "Home" , new { id = 1 });
        }
        if (!User.Identity!.IsAuthenticated)
            return Redirect("/Identity/Account/Login");

        var flow = _flowManager.GetFlow(flowStep.Flow.FlowId);
        var project = _projectManager.GetProject(flow.Project.ProjectId);
        bool userManager = User.Identity!.Name == project.SubPlatformAdmin.UserName;
        if (!(User.IsInRole("subplatformadministrator") || userManager))
            return Unauthorized();


        var data = PassDataInDto(flowStep);
        
        return View(data);
    }

    private object PassDataInDto(FlowStep currentFlowStep)
    {
        currentFlowStep = _flowStepManager.IdentifyFlowStep(currentFlowStep);
        var flow = _flowManager.GetFlowWithSubThemesExpectSelectedSubTheme(currentFlowStep.SubTheme);
        string flowStepType = currentFlowStep.GetType().Name;
        object responseData;

        
        switch (currentFlowStep)
        {
            case ClosedQuestion closedQuestion:
                ICollection<ConditionalPointDto> cqConditionalPointDtos = new List<ConditionalPointDto>();
                foreach (var conditionalPoint in closedQuestion.QuestionConditionalPoints)
                {
                    var cp = conditionalPoint.Criteria as ClosedAnswer;
                    var selectedAnswer = cp!.SelectedAnswer;
                    List<OptionDto> optionDtos = new List<OptionDto>();
                   
                    if (selectedAnswer != null)
                    {
                        optionDtos.Add(new OptionDto()
                        {
                            Id = (long)selectedAnswer,
                            Text = GetOptionTextForOption((long)selectedAnswer)
                        });
                        AnswerDto answerDto = new AnswerDto()
                        {
                            SelectedAnswer = (long)selectedAnswer,
                            Options = optionDtos
                        };
                        ConditionalPointDto conditionalPointDto = new ConditionalPointDto()
                        {
                            ConditionalPointId = conditionalPoint.ConditionalPointId,
                            Criteria = answerDto,
                            FollowUpStep = PassDataInDto(conditionalPoint.FollowUpStep) as FlowStepDtoWassim
                        };
                        cqConditionalPointDtos.Add(conditionalPointDto);
                    }
                }

                responseData = new FlowStepDtoWassim()
                {
                    FlowStepType = flowStepType,
                    Query = closedQuestion.Query,
                    Options = closedQuestion.Options,
                    SubthemeId = closedQuestion.SubTheme.SubthemeId,
                    Type = flowStepType,
                    SubthemeTitle = closedQuestion.SubTheme?.Title,
                    Ordernr = closedQuestion.OrderNr,
                    FlowStepId = closedQuestion.FlowStepId, OtherSubthemes = flow.Theme.SubThemes,
                    IsActive = closedQuestion.IsActive,
                    ConditionalPointDtos = cqConditionalPointDtos,
                    Flow = (int) flow.FlowId
                };
                break;
            case Info info:
                responseData = new FlowStepDtoWassim()
                {
                    FlowStepType = flowStepType,
                    Body = info.Body,
                    UploadedAudio = info.UrlAudio,
                    UploadedImage = info.UrlImage,
                    UploadedVideo = info.UrlVideo,
                    SubthemeId = info.SubTheme.SubthemeId,
                    Type = flowStepType,
                    SubthemeTitle = info.SubTheme?.Title,
                    Ordernr = info.OrderNr,
                    FlowStepId = info.FlowStepId,
                    OtherSubthemes = flow.Theme.SubThemes,
                    IsActive = info.IsActive
                    ,
                    Flow = (int) flow.FlowId
                };
                break;
            case MultipleChoiceQuestion multipleChoiceQuestion:
                ICollection<ConditionalPointDto> mcqConditionalPointDtos = new List<ConditionalPointDto>();
                foreach (var conditionalPoint in multipleChoiceQuestion.QuestionConditionalPoints)
                {
                    var cp = conditionalPoint.Criteria as MultipleChoiceAnswer;
                    List<OptionDto> options = new List<OptionDto>();

                    foreach (long id in cp!.SelectedAnswers)
                    {
                        options.Add(new OptionDto()
                        {
                            Id = id,
                            Text = GetOptionTextForOption(id)
                        });
                    }
                    AnswerDto answerDto = new AnswerDto()
                    {
                        SelectedAnswers = cp!.SelectedAnswers,
                        Options = options
                    };
                    ConditionalPointDto conditionalPointDto = new ConditionalPointDto()
                    {
                        ConditionalPointId = conditionalPoint.ConditionalPointId,

                        Criteria = answerDto,
                        FollowUpStep = PassDataInDto(conditionalPoint.FollowUpStep) as FlowStepDtoWassim
                    };
                    mcqConditionalPointDtos.Add(conditionalPointDto);
                }
                responseData = new FlowStepDtoWassim()
                {
                    FlowStepType = flowStepType,
                    Query = multipleChoiceQuestion.Query,
                    Options = multipleChoiceQuestion.Options,
                    SubthemeId = multipleChoiceQuestion.SubTheme.SubthemeId,
                    Type = flowStepType,
                    SubthemeTitle = multipleChoiceQuestion.SubTheme?.Title,
                    Ordernr = multipleChoiceQuestion.OrderNr,
                    FlowStepId = multipleChoiceQuestion.FlowStepId, OtherSubthemes = flow.Theme.SubThemes,
                    IsActive = multipleChoiceQuestion.IsActive,
                    ConditionalPointDtos = mcqConditionalPointDtos
                    ,
                    Flow = (int) flow.FlowId
                    
                };
                break;
            case OpenQuestion openQuestion:
                ICollection<ConditionalPointDto> oqConditionalPointDtos = new List<ConditionalPointDto>();
                foreach (var conditionalPoint in openQuestion.QuestionConditionalPoints)
                {
                    var cp = conditionalPoint.Criteria as OpenAnswer;
                    AnswerDto answerDto = new AnswerDto()
                    {
                        answer = cp!.Answer
                    };
                    ConditionalPointDto conditionalPointDto = new ConditionalPointDto()
                    {
                        ConditionalPointId = conditionalPoint.ConditionalPointId,
                        Criteria = answerDto,
                        FollowUpStep = PassDataInDto(conditionalPoint.FollowUpStep) as FlowStepDtoWassim
                    };
                    oqConditionalPointDtos.Add(conditionalPointDto);
                }
                responseData = new FlowStepDtoWassim()
                {
                    FlowStepType = flowStepType,
                    Query = openQuestion.Query,
                    SubthemeId = openQuestion.SubTheme.SubthemeId,
                    Type = flowStepType,
                    SubthemeTitle = openQuestion.SubTheme?.Title,
                    Ordernr = openQuestion.OrderNr,
                    FlowStepId = openQuestion.FlowStepId, OtherSubthemes = flow.Theme.SubThemes,
                    IsActive = openQuestion.IsActive,
                    ConditionalPointDtos = oqConditionalPointDtos
                    ,
                    Flow = (int) flow.FlowId
                };
                break;
            case RangeQuestion rangeQuestion:
                ICollection<ConditionalPointDto> rangeConditionalPointDtos = new List<ConditionalPointDto>();
                foreach (var conditionalPoint in rangeQuestion.QuestionConditionalPoints)
                {
                    var cp = conditionalPoint.Criteria as RangeAnswer;
                    AnswerDto answerDto = new AnswerDto()
                    {
                        SelectedAnswer = cp!.SelectedValue
                    };
                    ConditionalPointDto conditionalPointDto = new ConditionalPointDto()
                    {
                        ConditionalPointId = conditionalPoint.ConditionalPointId,
                        Criteria = answerDto,
                        FollowUpStep = PassDataInDto(conditionalPoint.FollowUpStep) as FlowStepDtoWassim
                    };
                    rangeConditionalPointDtos.Add(conditionalPointDto);
                }

                responseData = new FlowStepDtoWassim()
                {
                    FlowStepType = flowStepType,
                    Type = flowStepType,
                    MinValue = rangeQuestion.MinValue,
                    MaxValue = rangeQuestion.MaxValue,
                    Query = rangeQuestion.Query,
                    SubthemeId = rangeQuestion.SubTheme.SubthemeId,
                    SubthemeTitle = rangeQuestion.SubTheme?.Title,
                    Ordernr = rangeQuestion.OrderNr,
                    FlowStepId = rangeQuestion.FlowStepId, OtherSubthemes = flow.Theme.SubThemes,
                    IsActive = rangeQuestion.IsActive,
                    ConditionalPointDtos = rangeConditionalPointDtos
                    ,
                    Flow = (int) flow.FlowId
                };
                break;
            default:
                responseData = new
                {
                    FlowStepType = flowStepType,
                    flowStep = currentFlowStep
                };
                break;
        }

        return responseData;
    }

    private string GetOptionTextForOption(long id)
    { 
        return _flowStepManager.GetOptionTextForOption(id);
    }

    [HttpPost]
    public async Task<IActionResult> EditFlowStep(long id, [FromForm] FlowStepDtoWassim model)
    {
        var flowStep = _flowStepManager.GetFlowStep(id);
        Subtheme subtheme = _subthemeManager.GetSubtheme(model.SubthemeId);

        if (flowStep == null)
            return NotFound();

        await UpdateFlowStepBasedOnType(flowStep, model, subtheme);
        return RedirectToAction("Details", "Flow", new { flowid = flowStep.FlowId });
    }

    public async Task<IActionResult> DeleteAudio(long id)
    {
        var flowStep = _flowStepManager.GetFlowStep(id) as Info;
        if (flowStep == null)
            return NotFound();
        flowStep.UrlAudio = null;
        await _flowStepManager.UpdateInfo(flowStep);
        return RedirectToAction("Edit", new { id = id });
    }
    public async Task<IActionResult> DeleteVideo(long id)
    {
        var flowStep = _flowStepManager.GetFlowStep(id) as Info;
        if (flowStep == null)
            return NotFound();
        flowStep.UrlVideo = null;
        await _flowStepManager.UpdateInfo(flowStep);
        return RedirectToAction("Edit", new { id = id });
    }
    public async Task<IActionResult> DeleteImage(long id)
    {
        var flowStep = _flowStepManager.GetFlowStep(id) as Info;
        if (flowStep == null)
            return NotFound();
        flowStep.UrlImage = null;
        await _flowStepManager.UpdateInfo(flowStep);
        return RedirectToAction("Edit", new { id = id });
    }


    public async Task UpdateFlowStepBasedOnType(FlowStep flowStep, FlowStepDtoWassim model, Subtheme subtheme)
    {
        flowStep.Header = model.Header;
        flowStep.IsActive = model.IsActive;

        switch (flowStep)
        {
            case ClosedQuestion closedQuestion:
                closedQuestion.Query = model.Query;
                closedQuestion.Options = model.Options;
                closedQuestion.SubTheme = subtheme;
                closedQuestion.IsActive = model.IsActive;
                _flowStepManager.UpdateClosedQuestion(closedQuestion);
                break;
            case Info info:
                if (model.Image != null)
                {
                    string urlInfoPicture = await _cloudBucketManager.UploadFile(model.Image, info.FlowStepId, "Info");
                    info.UrlImage = urlInfoPicture;
                }

                if (model.Video != null)
                {
                    string urlInfoVideo = await _cloudBucketManager.UploadFile(model.Video, info.FlowStepId, "Info");
                    info.UrlVideo = urlInfoVideo;
                }

                if (model.Audio != null)
                {
                    string urlInfoAudio = await _cloudBucketManager.UploadFile(model.Audio, info.FlowStepId, "Info");
                    info.UrlAudio = urlInfoAudio;
                }

                info.Body = model.Body;
                info.SubTheme = subtheme;
                info.IsActive = model.IsActive;
                await _flowStepManager.UpdateInfo(info);
                break;
            case MultipleChoiceQuestion multipleChoiceQuestion:
                multipleChoiceQuestion.Query = model.Query;
                multipleChoiceQuestion.Options = model.Options;
                multipleChoiceQuestion.SubTheme = subtheme;
                multipleChoiceQuestion.IsActive = model.IsActive;
                _flowStepManager.UpdateMultipleChoiceQuestion(multipleChoiceQuestion);
                break;
            case OpenQuestion openQuestion:
                openQuestion.Query = model.Query;
                openQuestion.SubTheme = subtheme;
                openQuestion.IsActive = model.IsActive;
                _flowStepManager.UpdateOpenQuestion(openQuestion);
                break;
            case RangeQuestion rangeQuestion:
                rangeQuestion.MinValue = model.MinValue;
                rangeQuestion.MaxValue = model.MaxValue;
                rangeQuestion.Query = model.Query;
                rangeQuestion.SubTheme = subtheme;
                rangeQuestion.IsActive = model.IsActive;
                _flowStepManager.UpdateRangeQuestion(rangeQuestion);
                break;
        }
    }


    public IActionResult AddConditionalPoint(long flowstepId)
    {
        var flowstep = _flowStepManager.GetFlowStep(flowstepId);
        flowstep = _flowStepManager.GetQuestionWithOptions(flowstep as Question);
        string flowStepType = "";
        FlowStepDtoWassim flowstepdto = new FlowStepDtoWassim();
        if (flowstep is RangeQuestion)
        {
            RangeQuestion flowstepRq = flowstep as RangeQuestion;
            flowstepdto.MinValue = flowstepRq.MinValue;
            flowstepdto.MinValue = flowstepRq.MinValue;
            flowStepType = nameof(flowstepRq);
        }
        else if (flowstep is MultipleChoiceQuestion)
        {
            MultipleChoiceQuestion flowstepMcq = flowstep as MultipleChoiceQuestion;
            flowstepdto.Options = flowstepMcq.Options;
            flowStepType = nameof(flowstepMcq);
        }
        else if (flowstep is ClosedQuestion)
        {
            ClosedQuestion flowstepCq = flowstep as ClosedQuestion;
            flowstepdto.Options = flowstepCq.Options;

            flowStepType = nameof(flowstepCq);
        }

        flowstepdto.FlowStepId = flowstepId;
        flowstepdto.Header = flowstep.Header;
        CreateConditionalPointViewModel conditionalPointViewModel = new CreateConditionalPointViewModel()
        {
            Query = flowstep.Header,
            FlowStepId = flowstepId,
            FlowStepType = flowStepType
        };
        return View(conditionalPointViewModel);
    }
    
}