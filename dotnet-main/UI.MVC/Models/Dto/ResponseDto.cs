using UI.MVC.Models.Dto.AnswerDto;

namespace IP.Models.Dto;

public class ResponseDto
{
    public AnswerDto Answer { get; set; }
    public QuestionDto Question { get; set; }
}