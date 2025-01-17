using IP.Models.Dto;

namespace UI.MVC.Models.Dto.AnswerDto;

public class AnswerDto
{
    public ResponseDto Response { get; set; }
    public long SelectedAnswer { get; set; }
    public string answer { get; set; }
    public List<long> SelectedAnswers { get; set; }
    public string AnswerType { get; set; }
    public List<OptionDto> Options { get; set; }
    public long? SelectedValue { get; set; }
}