using System.ComponentModel.DataAnnotations;
using IP.Models.Dto;
using PIP.Domain.Flow;

namespace UI.MVC.Models.Dto;

public class ClosedQuestionDto : FlowStepDto
{
    [Required(ErrorMessage = "Opties zijn verplicht.")]
    public ICollection<OptionDto> Options { get; set; } = new List<OptionDto>();
}