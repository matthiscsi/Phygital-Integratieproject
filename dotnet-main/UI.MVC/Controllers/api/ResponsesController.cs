using System.Security.Claims;
using System.Text.RegularExpressions;
using IP.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PIP.BL.IManagers;
using PIP.BL.Managers;
using PIP.Domain.Flow;
using PIP.Domain.Flow.Inquiry;
using UI.MVC.Models.Dto;
using UI.MVC.Models.Dto.AnswerDto;

namespace UI.MVC.Controllers.api;

[Route("api/[controller]")]
[ApiController]
public class ResponsesController : ControllerBase
{
    private readonly IFlowStepManager _flowStepManager;
    private readonly IResponseManager _responsemanager;
    private readonly IFlowSessionManager _flowSessionManager;
    private readonly IProjectManager _projectManager;
    private readonly UnitOfWork _uof;


    public ResponsesController(IFlowStepManager flowStepManager, IResponseManager responsemanager, IFlowSessionManager flowSessionManager, IProjectManager projectManager, UnitOfWork uof)
    {
        _flowStepManager = flowStepManager;
        _responsemanager = responsemanager;
        _flowSessionManager = flowSessionManager;
        _projectManager = projectManager;
        _uof = uof;
    }

    [HttpPost("addanswer")]
    public IActionResult AddAnswer([FromBody] AnswerRequest answerRequest)
    {
        _uof.BeginTransaction();

        Response response;
        Answer sentAnswer;
        Question sentQuestion;

        if (answerRequest == null) return BadRequest("Answer not provided");

        var flowSession = _flowSessionManager.GetFlowSession(answerRequest.FlowSessionId);
        FlowStep flowStep = flowSession.CurrentFlowStep;
        
        if (flowStep is ClosedQuestion)
        {
            sentAnswer = new ClosedAnswer { SelectedAnswer = answerRequest.SelectedAnswer };
            sentQuestion = (ClosedQuestion)flowStep;
        }
        else if (flowStep is OpenQuestion)
        {
            sentAnswer = new OpenAnswer { Answer = answerRequest.Answer };
            sentQuestion = (OpenQuestion)flowStep;
        }
        else if (flowStep is MultipleChoiceQuestion)
        {
            sentAnswer = new MultipleChoiceAnswer { SelectedAnswers = answerRequest.SelectedAnswers };
            sentQuestion = (MultipleChoiceQuestion)flowStep;
        }
        else if (flowStep is RangeQuestion)
        {
            sentAnswer = new RangeAnswer { SelectedValue = answerRequest.SelectedValue };
            sentQuestion = (RangeQuestion)flowStep;
        }
        else
        {
            return RedirectToAction("Index", "FlowStep",
                new { id = flowSession.FlowSessionId });
        }
        
        response = _responsemanager.AddResponse(sentAnswer, sentQuestion, flowSession);
        _uof.Commit();
        return Ok(response);
    }
    [HttpGet("ClosedQuestionAnswersData/{id:long}")]
    [Authorize(Roles = "subplatformadministrator")]
    public IActionResult GetAnswersDataForClosedQuestion(long id)
    {     var project = _projectManager.ReadProjectFromFlowstep(id);
        if (!User.Identity!.IsAuthenticated)
        {
            return Redirect("/Identity/Account/Login");
        }
             var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
     
             if (project.SubPlatformAdmin.Id != userId)
             {
                 return   Redirect("/Identity/Account/AccessDenied");
                 
             }
        //refactoren
        List<ClosedAnswerAnalysisDto> closedAnswerAnalysisDtos = new List<ClosedAnswerAnalysisDto>();

        ClosedQuestion flowStep = _flowStepManager.GetClosedQuestionWithOptions(id);

   
        foreach (Option option in flowStep.Options)
        {
            closedAnswerAnalysisDtos.Add(new ClosedAnswerAnalysisDto()
            {
                Quantity = _responsemanager.GetResponseCountForClosedOption(option.Id),
                ClosedAnswerDto = new ClosedAnswerDto()
                {
                    SelectedAnswer = option.Id,
                    SelectedAnswerText = option.Text
                }
            });
        }

        return Ok(closedAnswerAnalysisDtos);
    }
    [HttpGet("MultipleChoiceQuestionAnswersData/{id:long}")]
    [Authorize(Roles = "subplatformadministrator")]
    public IActionResult GetAnswersDataForMultipleChoiceQuestion(long id)
    {   var project = _projectManager.ReadProjectFromFlowstep(id);
        if (!User.Identity!.IsAuthenticated)
        {
            return Redirect("/Identity/Account/Login");
        }
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
     
        if (project.SubPlatformAdmin.Id != userId)
        {
            return   Redirect("/Identity/Account/AccessDenied");
                 
        }
        //refactoren
        List<MultipleChoiceAnswerAnalysisDto> multipleChoiceAnswerAnalysisDtos = new List<MultipleChoiceAnswerAnalysisDto>();
        //object mapper nuget package
        MultipleChoiceQuestion flowStep = _flowStepManager.GetMultipleChoiceQuestionWithOptions(id);

        foreach (Option option in flowStep.Options)
        {
            multipleChoiceAnswerAnalysisDtos.Add(new MultipleChoiceAnswerAnalysisDto()
            {
                Quantity = _responsemanager.GetResponseCountForMultipleChoiceOption(option.Id),
                MultipleChoiceAnswerDto = new MultipleChoiceAnswerDto()
                {
                    SelectedAnswer = option.Id,
                    SelectedAnswerText = option.Text
                }
            });
        }

        return Ok(multipleChoiceAnswerAnalysisDtos);
    }
    [HttpGet("OpenQuestionAnswersData/{id:long}")]
    [Authorize(Roles = "subplatformadministrator")]
    public IActionResult GetAnswersDataForOpenQuestion(long id)
    {   var project = _projectManager.ReadProjectFromFlowstep(id);
        if (!User.Identity!.IsAuthenticated)
        {
            return Redirect("/Identity/Account/Login");
        }
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
     
        if (project.SubPlatformAdmin.Id != userId)
        {
            return   Redirect("/Identity/Account/AccessDenied");
                 
        }
        var answers = _responsemanager.GetAnswersForOpenQuestion(id);
        List<string> answerStrings = new List<string>();

        foreach (OpenAnswer openAnswer in answers)
        {
            var cleanAnswer = Regex.Replace(openAnswer.Answer.ToLower(), @"[^a-z ]", "");
            answerStrings.Add(cleanAnswer);
        }

        var wordFrequency = new Dictionary<string, int>();
        foreach (var answerString in answerStrings)
        {
            var words = answerString.Split(' ');

            foreach (var word in words)
            {
                if (!string.IsNullOrWhiteSpace(word))
                {
                    if (!wordFrequency.ContainsKey(word))
                    {
                        wordFrequency[word] = 1;
                    }
                    else
                    {
                        wordFrequency[word]++;
                    }
                }
            }
        }

        var wordCloudData = wordFrequency.Select(pair => new { Word = pair.Key, Frequency = pair.Value }).ToList();

        return Ok(wordCloudData);
    }
    
    
  
    
    [HttpGet("RangeQuestionAnswersData/{id:long}")]
    [Authorize(Roles = "subplatformadministrator")]
    public IActionResult GetAnswersDataForRangeQuestion(long id)
    {
        var project = _projectManager.ReadProjectFromFlowstep(id);
        if (!User.Identity!.IsAuthenticated)
        {
            return Redirect("/Identity/Account/Login");
        }
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
     
        if (project.SubPlatformAdmin.Id != userId)
        {
            return   Redirect("/Identity/Account/AccessDenied");
                 
        }
        RangeQuestion rangeQuestion = _flowStepManager.GetRangeQuestion(id);
        if (rangeQuestion == null)
        {
            return NotFound();
        }

        List<RangeAnswerAnalysisDto> rangeAnswerAnalysisDtos = new List<RangeAnswerAnalysisDto>();

        var answers = _responsemanager.GetAnswersForRangeQuestion(id);
        
        var answerCounts = answers.GroupBy(a => a.SelectedValue)
            .Select(g => new { Value = g.Key, Count = g.Count() })
            .OrderBy(x => x.Value)
            .ToList();

        foreach (var item in answerCounts)
        {
            rangeAnswerAnalysisDtos.Add(new RangeAnswerAnalysisDto
            {
                RangeAnswerDto = new RangeAnswerDto
                {
                    SelectedAnswer = item.Value
                },
                Quantity = item.Count
            });
        }

        return Ok(rangeAnswerAnalysisDtos);
    }
    [HttpGet("TotalResponseCount/{id:long}")]
    public IActionResult GetResponseCountForQuestion(long id)
    {
        int responseCount = _responsemanager.GetResponseCountForQuestion(id);
        return Ok(responseCount);
    }

    [HttpGet("OpenAnswers/{id:long}")]
    public IActionResult GetOpenAnswersForOpenQuestion(long id)
    {
        var answers = _responsemanager.GetAnswersForOpenQuestion(id);
        List<string> answerData = new List<string>();
        
        foreach (OpenAnswer answer in answers)
        {
            answerData.Add(answer.Answer);
        }

        return Ok(answerData);
    }
}

