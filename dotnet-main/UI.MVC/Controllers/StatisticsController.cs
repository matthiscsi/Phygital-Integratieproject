using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using CsvHelper;
using IP.Models.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PIP.BL.IManagers;
using PIP.Domain.Deelplatform;
using PIP.Domain.Flow;
using PIP.Domain.Flow.Inquiry;
using UI.MVC.Models.Dto;

namespace UI.MVC.Controllers;

public class StatisticsController : Controller
{
    private readonly IFlowManager _flowManager;
    private readonly IFlowStepManager _flowStepManager;
    private readonly FlowStepHelper _flowStepHelper;
    private readonly IProjectManager _projectManager;
    private readonly IFlowSessionManager _flowSessionManager;
    private readonly ISubPlatformManager _subplatformManager;
    private readonly IResponseManager _responsemanager;

    public StatisticsController(IFlowManager flowManager, IFlowStepManager flowStepManager,
        FlowStepHelper flowStepHelper, IProjectManager projectManager, IFlowSessionManager flowSessionManager,
        ISubPlatformManager subplatformManager, IResponseManager responsemanager)
    {
        _flowManager = flowManager;
        _flowStepManager = flowStepManager;
        _flowStepHelper = flowStepHelper;
        _projectManager = projectManager;
        _flowSessionManager = flowSessionManager;
        _subplatformManager = subplatformManager;
        _responsemanager = responsemanager;
    }

    [Authorize(Roles = "subplatformadministrator")]
    public IActionResult FlowAnalysis(long flowId)
    {
        var flow = _flowManager.GetFlowWithProject(flowId);
        if (flow == null)
            return RedirectToAction("Index", "Home");
        if (!User.Identity!.IsAuthenticated)
            return Redirect("/Identity/Account/Login");


        bool userManager = User.Identity.Name == flow.Project.SubPlatformAdmin.UserName;
        if (!userManager)
        {
            return Unauthorized();
        }

        FlowDto flowDto = new FlowDto()
        {
            Description = flow.Description,
            Id = flow.FlowId,
            ProjectName = flow.Project.Name,
            Title = flow.Title,
            UrlPhoto = flow.UrlFlowPicture
        };
        return View(flowDto);
    }


    [Authorize(Roles = "subplatformadministrator")]
    public IActionResult FlowStepAnalysis(long flowStepId)
    {
        var flowStep = _flowStepManager.GetFlowStep(flowStepId);
        if (flowStep == null)
            return RedirectToAction("Index", "Home");
        var flowFromFlowstep = _flowManager.GetFlowFromFlowstep(flowStepId);


        var flow = _flowManager.GetFlowWithProject(flowFromFlowstep.FlowId);
        if (!User.Identity!.IsAuthenticated)
            return Redirect("/Identity/Account/Login");


        bool userManager = User.Identity.Name == flow.Project.SubPlatformAdmin.UserName;
        if (!userManager)
        {
            return Unauthorized();
        }

        FlowStepDto flowStepDto = _flowStepHelper.PassDataInDto(flowStep);
        switch (flowStepDto)
        {
            case ClosedQuestionDto closedQuestion:
                return View("ClosedQuestionAnalysis", closedQuestion);

            case MultipleChoiceQuestionDto multipleChoiceQuestion:
                return View("MultipleChoiceQuestionAnalysis", multipleChoiceQuestion);

            case OpenQuestionDto openQuestion:
                return View("OpenQuestionAnalysis", openQuestion);

            case RangeQuestionDto rangeQuestion:
                return View("RangeQuestionAnalysis", rangeQuestion);
        }

        return null;
    }

    [Authorize(Roles = "subplatformadministrator")]
    public IActionResult ProjectAnalysis(long id)
    {
        var project = _projectManager.GetProject(id);
        if (project == null)
            return RedirectToAction("Index", "Home");

        if (!User.Identity!.IsAuthenticated)
            return Redirect("/Identity/Account/Login");

        bool userManager = User.Identity.Name == project.SubPlatformAdmin.UserName;
        if (!userManager)
        {
            return Unauthorized();
        }

        ProjectDto projectDto = new ProjectDto()
        {
            ProjectId = project.ProjectId,
            Name = project.Name,
            Description = project.Description
        };
        return View(projectDto);
    }


    [Authorize(Roles = "admin")]
    public IActionResult SubplatformAnalysis(long id)
    {
        var subplatform = _subplatformManager.GetSubplatform(id);
        if (subplatform == null)
        {
            TempData["RedirectMessage"] = "Dit subplatform bestaat niet.";
            return RedirectToAction("Index", "Home");
        }


        if (!User.Identity!.IsAuthenticated)
            return Redirect("/Identity/Account/Login");

        var subplatformDto = new SubplatformDto
        {
            MainText = subplatform.MainText,
            CustomerName = subplatform.CustomerName,
            Id = subplatform.SubplatformId
        };
        return View(subplatformDto);
    }

    [Authorize(Roles = "admin")]
    public IActionResult ExportSubplatformDataToCsv(long subplatformId)
    {
        var subplatform = _subplatformManager.GetSubplatformWithProjects(subplatformId);
        var records = new List<dynamic>();


        if (!User.Identity!.IsAuthenticated)
        {
            return Redirect("/Identity/Account/Login");
        }

   

        foreach (Project project in subplatform.Projects)
        {
            var flowSessionCountOfProject =
                _projectManager.GetFlowSessionCountOfProject(project.ProjectId);
            var averageTimeSpentForProject =
                _projectManager.GetAverageTimeSpentForFlowsOfProject(project.ProjectId);

            records.Add(new
            {
                SubplatformId = subplatformId,
                ProjectId = project.ProjectId,
                ProjectName = project.Name,
                FlowSessionCountProject = flowSessionCountOfProject,
                AverageTimeSpentForProject = averageTimeSpentForProject
            });
        }

        using (var memoryStream = new MemoryStream())
        using (var streamWriter = new StreamWriter(memoryStream, Encoding.UTF8))
        using (var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture))
        {
            csvWriter.WriteRecords(records);
            streamWriter.Flush();
            return File(memoryStream.ToArray(), "text/csv",
                subplatform.CustomerName + "SubplatformData.csv");
        }
    }

    [Authorize(Roles = "subplatformadministrator")]
    public IActionResult ExportProjectDataToCsv(long projectId)
    {
        var project = _projectManager.GetProject(projectId);
        if (!User.Identity!.IsAuthenticated)
        {
            return Redirect("/Identity/Account/Login");
        }


        bool userManager = User.Identity.Name == project.SubPlatformAdmin.UserName;
        if (!userManager)
        {
            return Unauthorized();
        }

        var flows = _flowManager.GetFlowsOfProject(projectId);

        var records = new List<dynamic>();
        foreach (var flow in flows)
        {
            var flowSessionCountOfProject = _projectManager.GetFlowSessionCountOfProject(projectId);
            var flowSessionCountOfFlow = _flowSessionManager.GetFlowSessionCount(flow.FlowId);
            records.Add(new
            {
                ProjectId = project.ProjectId, ProjectName = project.Name,
                FlowSessionCountProject = flowSessionCountOfProject, FlowId = flow.FlowId,
                FlowSessionCountOfFlow = flowSessionCountOfFlow,
                AverageTime = _flowSessionManager.GetAverageTimeSpentForFlow(flow.FlowId)
            });
        }

        using (var memoryStream = new MemoryStream())
        using (var streamWriter = new StreamWriter(memoryStream, Encoding.UTF8))
        using (var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture))
        {
            csvWriter.WriteRecords(records);
            streamWriter.Flush();
            return File(memoryStream.ToArray(), "text/csv", project.Name + "ProjectData.csv");
        }
    }

    public IActionResult ExportFlowDataToCsv(long flowId)
    {
        if (!User.Identity!.IsAuthenticated)
        {
            return Redirect("/Identity/Account/Login");
        }

        var project = _projectManager.ReadProjectFromFlow(flowId);
        bool userManager = User.Identity.Name == project.SubPlatformAdmin.UserName;
        if (!userManager)
        {
            return Unauthorized();
        }

        var flow = _flowManager.GetFlow(flowId);
        var subthemes = _flowManager.GetSubthemesOfFlow(flowId);

        var records = new List<dynamic>();

        var flowTitle = flow.Title;
        var flowSessionCount = _flowSessionManager.GetFlowSessionCount(flowId);
        var flowSessionAverageTime = _flowSessionManager.GetAverageTimeSpentForFlow(flowId);
        foreach (var subtheme in subthemes)
        {
            var subthemeSkippedCount =
                _flowSessionManager.GetSubthemeSkippedCount(flowId, subtheme.SubthemeId);
            records.Add(new
            {
                FlowId = flowId,
                FlowTitle = flowTitle,
                FlowSessionCount = flowSessionCount,
                FlowSessionAverageTime = flowSessionAverageTime,
                SubthemeId = subtheme.SubthemeId,
                SubthemeTitle = subtheme.Title,
                SubthemeSkippedCount = subthemeSkippedCount
            });
        }

        using (var memoryStream = new MemoryStream())
        using (var streamWriter = new StreamWriter(memoryStream, Encoding.UTF8))
        using (var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture))
        {
            csvWriter.WriteRecords(records);
            streamWriter.Flush();
            return File(memoryStream.ToArray(), "text/csv", flow.Title + "FlowData.csv");
        }
    }

    public IActionResult ExportClosedQuestionDataToCsv(long flowStepId)
    {
        if (!User.Identity!.IsAuthenticated)
        {
            return Redirect("/Identity/Account/Login");
        }

        var project = _projectManager.ReadProjectFromFlowstep(flowStepId);
        bool userManager = User.Identity.Name == project.SubPlatformAdmin.UserName;
        if (!userManager)
        {
            return Unauthorized();
        }

        ClosedQuestion closedQuestion = _flowStepManager.GetClosedQuestionWithOptions(flowStepId);
        var flow = _flowManager.GetFlow(closedQuestion.FlowId);


        var records = new List<dynamic>();

        foreach (Option option in closedQuestion.Options)
        {
            var optionResponseCount = _responsemanager.GetResponseCountForClosedOption(option.Id);
            var totalResponseCount = _responsemanager.GetResponseCountForQuestion(flowStepId);

            records.Add(new
            {
                FlowId = flow.FlowId,
                QuestionId = flowStepId,
                Question = closedQuestion.Query,
                TotalResponseCount = totalResponseCount,
                Option = option.Text,
                OptionSelectedCount = optionResponseCount,
            });
        }

        using (var memoryStream = new MemoryStream())
        using (var streamWriter = new StreamWriter(memoryStream, Encoding.UTF8))
        using (var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture))
        {
            csvWriter.WriteRecords(records);
            streamWriter.Flush();
            return File(memoryStream.ToArray(), "text/csv",
                "FlowStep" + closedQuestion.FlowStepId + "DataOfFlow" + flow.Title + ".csv");
        }
    }


    public IActionResult ExportRangeQuestionDataToCsv(long flowStepId)
    {
        RangeQuestion rangeQuestion = _flowStepManager.GetRangeQuestion(flowStepId);
        var flow = _flowManager.GetFlow(rangeQuestion.FlowId);


        if (!User.Identity!.IsAuthenticated)
        {
            return Redirect("/Identity/Account/Login");
        }

        var project = _projectManager.ReadProjectFromFlowstep(flowStepId);
        bool userManager = User.Identity.Name == project.SubPlatformAdmin.UserName;
        if (!userManager)
        {
            return Unauthorized();
        }

        var records = new List<dynamic>();
        IEnumerable<RangeAnswer> rangeAnswers = _responsemanager.GetAnswersForRangeQuestion(flowStepId);
        Dictionary<long, int> answerCounts = new Dictionary<long, int>();
        for (int i = rangeQuestion.MinValue; i <= rangeQuestion.MaxValue; i++)
        {
            answerCounts.Add(i, 0);
        }

        foreach (var answer in rangeAnswers)
        {
            if (answerCounts.ContainsKey(answer.SelectedValue))
            {
                answerCounts[answer.SelectedValue]++;
            }
        }

        var totalResponseCount = _responsemanager.GetResponseCountForQuestion(flowStepId);

        foreach (var kvp in answerCounts)
        {
            records.Add(new
            {
                FlowId = flow.FlowId,
                QuestionId = flowStepId,
                Question = rangeQuestion.Query,
                TotalResponseCount = totalResponseCount,
                Answer = kvp.Key,
                Count = kvp.Value
            });
        }

        using (var memoryStream = new MemoryStream())
        using (var streamWriter = new StreamWriter(memoryStream, Encoding.UTF8))
        using (var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture))
        {
            csvWriter.WriteRecords(records);
            streamWriter.Flush();
            return File(memoryStream.ToArray(), "text/csv",
                "FlowStep" + rangeQuestion.FlowStepId + "DataOfFlow" + flow.Title + ".csv");
        }
    }

    public IActionResult ExportOpenQuestionDataToCsv(long flowStepId)
    {
        var openAnswers = _responsemanager.GetAnswersForOpenQuestion(flowStepId);
        if (!User.Identity!.IsAuthenticated)
        {
            return Redirect("/Identity/Account/Login");
        }

        var project = _projectManager.ReadProjectFromFlowstep(flowStepId);
        bool userManager = User.Identity.Name == project.SubPlatformAdmin.UserName;
        if (!userManager)
        {
            return Unauthorized();
        }

        var wordFrequency = new Dictionary<string, int>();
        var totalResponseCount = _responsemanager.GetResponseCountForQuestion(flowStepId);

        // Count word frequency
        foreach (var openAnswer in openAnswers)
        {
            var words = Regex.Matches(openAnswer.Answer.ToLower(), @"\b[a-z]+\b");

            foreach (Match match in words)
            {
                var word = match.Value;
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

        var records = wordFrequency.Select(pair => new
        {
            QuestionId = flowStepId, TotalResponseCount = totalResponseCount,
            Word = pair.Key, Frequency = pair.Value
        }).ToList();

        using (var memoryStream = new MemoryStream())
        using (var streamWriter = new StreamWriter(memoryStream, Encoding.UTF8))
        using (var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture))
        {
            csvWriter.WriteRecords(records);
            streamWriter.Flush();
            return File(memoryStream.ToArray(), "text/csv",
                "FlowStep" + flowStepId + "OpenQuestionData.csv");
        }
    }

    public IActionResult ExportMultipleChoiceQuestionDataToCsv(long flowStepId)
    {
        MultipleChoiceQuestion multipleChoiceQuestion =
            _flowStepManager.GetMultipleChoiceQuestionWithOptions(flowStepId);
        var flow = _flowManager.GetFlow(multipleChoiceQuestion.FlowId);
        var records = new List<dynamic>();
        if (!User.Identity!.IsAuthenticated)
        {
            return Redirect("/Identity/Account/Login");
        }

        var project = _projectManager.ReadProjectFromFlowstep(flowStepId);
        bool userManager = User.Identity.Name == project.SubPlatformAdmin.UserName;
        if (!userManager)
        {
            return Unauthorized();
        }

        foreach (Option option in multipleChoiceQuestion.Options)
        {
            var optionResponseCount =
                _responsemanager.GetResponseCountForMultipleChoiceOption(option.Id);
            var totalResponseCount = _responsemanager.GetResponseCountForQuestion(flowStepId);
            records.Add(new
            {
                FlowId = flow.FlowId,
                QuestionId = flowStepId,
                Question = multipleChoiceQuestion.Query,
                TotalResponseCount = totalResponseCount,
                Answer = option.Text,
                optionResponseCount = optionResponseCount
            });
        }

        using (var memoryStream = new MemoryStream())
        using (var streamWriter = new StreamWriter(memoryStream, Encoding.UTF8))
        using (var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture))
        {
            csvWriter.WriteRecords(records);
            streamWriter.Flush();
            return File(memoryStream.ToArray(), "text/csv",
                "FlowStep" + multipleChoiceQuestion.FlowStepId + "DataOfFlow" + flow.Title + ".csv");
        }
    }


    public IActionResult ExportOpenAnswersToCsv(long flowStepId)
    {
        var openAnswers = _responsemanager.GetAnswersForOpenQuestion(flowStepId);
        FlowStep openQuestion = _flowStepManager.GetFlowStep(flowStepId) as OpenQuestion;
        var flow = _flowManager.GetFlow(openQuestion.FlowId);
        var totalResponseCount = _responsemanager.GetResponseCountForQuestion(flowStepId);
        var records = new List<dynamic>();

        foreach (var openAnswer in openAnswers)
        {
            records.Add(new
            {
                FlowId = flow.FlowId,
                QuestionId = flowStepId,
                TotalResponseCount = totalResponseCount,
                Answer = openAnswer.Answer,
                Date = openAnswer.Response.FlowSession.SessionEndDate.Date
            });
        }


        using (var memoryStream = new MemoryStream())
        using (var streamWriter = new StreamWriter(memoryStream, Encoding.UTF8))
        using (var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture))
        {
            csvWriter.WriteRecords(records);
            streamWriter.Flush();
            return File(memoryStream.ToArray(), "text/csv",
                "FlowStep" + flowStepId + "OpenQuestionAnswers.csv");
        }
    }
}