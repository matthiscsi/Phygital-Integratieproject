using PIP.Domain.Flow.Inquiry;
using UI.MVC.Models.Dto;

namespace IP.Models.Dto;

public class QuestionDto : FlowStepDto
{
    public string Query { get; set; }
    public ICollection<ResponseDto> Responses { get; set; }
    public List<Option> Options { get; set; }
}