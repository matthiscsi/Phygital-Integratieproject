using IP.Models.Dto;
using UI.MVC.Models.Dto;
using UI.MVC.Models.Dto.AnswerDto;

namespace UI.MVC.Models;

public class ConditionalPointDto
{
    public long ConditionalPointId { get; set; }
    public FlowStepDtoWassim Question { get; set; }
    public FlowStepDtoWassim FollowUpStep { get; set; }
    public AnswerDto Criteria { get; set; }

}