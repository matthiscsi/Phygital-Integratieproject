using IP.Models.Dto;
using PIP.BL.IManagers;
using PIP.Domain.Flow;
using PIP.Domain.Flow.Inquiry;
using UI.MVC.Models.Dto;

namespace UI.MVC.Controllers;

public class FlowStepHelper
{
    private readonly IFlowStepManager _flowStepManager;

    public FlowStepHelper(IFlowStepManager flowStepManager)
    {
        _flowStepManager = flowStepManager;
    }

    public FlowStepDto PassDataInDto(FlowStep flowStep)
    {
        flowStep = _flowStepManager.IdentifyFlowStep(flowStep);

        FlowStepDto flowStepDto = flowStep switch
        {
            ClosedQuestion closedQuestion => new ClosedQuestionDto
            {
                Query = closedQuestion.Query,
                Subtheme = closedQuestion.SubTheme?.Title,
                OrderNr = closedQuestion.OrderNr,
                Options = closedQuestion.Options.Select(option => new OptionDto
                {
                    Id = option.Id,
                    Text = option.Text
                }).ToList(),
                FlowId = closedQuestion.FlowId,
                IsActive = closedQuestion.IsActive,
                FlowStepId = closedQuestion.FlowStepId,
                FlowStepType = flowStep.GetType().Name,
                SubthemeId = closedQuestion.SubTheme!.SubthemeId
            },
            Info info => new InfoDto
            {
                Query = info.Header,
                Subtheme = info.SubTheme?.Title,
                OrderNr = info.OrderNr,
                FlowId = info.FlowId,
                IsActive = info.IsActive,
                FlowStepId = info.FlowStepId,
                Body = info.Body,
                FlowStepType = flowStep.GetType().Name,
                SubthemeId = info.SubTheme!.SubthemeId,
                UrlVideo = info.UrlVideo,
                UrlAudio = info.UrlAudio,
                UrlImage = info.UrlImage
                
            },
            MultipleChoiceQuestion multipleChoiceQuestion => new MultipleChoiceQuestionDto
            {
                Query = multipleChoiceQuestion.Query,
                Subtheme = multipleChoiceQuestion.SubTheme?.Title,
                OrderNr = multipleChoiceQuestion.OrderNr,
                FlowId = multipleChoiceQuestion.FlowId,
                IsActive = multipleChoiceQuestion.IsActive,
                FlowStepId = multipleChoiceQuestion.FlowStepId,
                Options = multipleChoiceQuestion.Options.Select(option => new OptionDto
                {
                    Id = option.Id,
                    Text = option.Text
                }).ToList(),
                FlowStepType = flowStep.GetType().Name,
                SubthemeId = multipleChoiceQuestion.SubTheme!.SubthemeId
            },
            OpenQuestion openQuestion => new OpenQuestionDto
            {
                Query = openQuestion.Query,
                Subtheme = openQuestion.SubTheme?.Title,
                OrderNr = openQuestion.OrderNr,
                FlowId = openQuestion.FlowId,
                IsActive = openQuestion.IsActive,
                FlowStepId = openQuestion.FlowStepId,
                FlowStepType = flowStep.GetType().Name,
                SubthemeId = openQuestion.SubTheme!.SubthemeId
            },
            RangeQuestion rangeQuestion => new RangeQuestionDto
            {
                Query = rangeQuestion.Query,
                Subtheme = rangeQuestion.SubTheme?.Title,
                OrderNr = rangeQuestion.OrderNr,
                FlowId = rangeQuestion.FlowId,
                IsActive = rangeQuestion.IsActive,
                FlowStepId = rangeQuestion.FlowStepId,
                MaxValue = rangeQuestion.MaxValue,
                MinValue = rangeQuestion.MinValue,
                FlowStepType = flowStep.GetType().Name,
                SubthemeId = rangeQuestion.SubTheme!.SubthemeId
            },
            _ => null
        };

        return flowStepDto;
    }
}