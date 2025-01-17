using System.ComponentModel.DataAnnotations;
using IP.Models.Dto;

namespace UI.MVC.Models.Dto;

public class RangeQuestionDto : FlowStepDto
{
    [Required(ErrorMessage = "Minimum waarde is verplicht.")]
    public int MinValue { get; set; }

    [Required(ErrorMessage = "Maximum waarde is verplicht.")]
    public int MaxValue { get; set; }
}