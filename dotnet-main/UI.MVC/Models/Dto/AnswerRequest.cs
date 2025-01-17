#nullable enable
namespace UI.MVC.Models.Dto;

public class AnswerRequest
{
    public int OrderNr { get; set; }
    public long FlowSessionId { get; set; }
    public long? SelectedAnswer { get; set; }
    public List<long>? SelectedAnswers { get; set; }

    public long SelectedValue { get; set; }

    public string? Answer { get; set; }
}