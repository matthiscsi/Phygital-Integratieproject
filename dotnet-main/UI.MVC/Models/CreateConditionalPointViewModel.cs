namespace UI.MVC.Models;

public class CreateConditionalPointViewModel
{
    public long FlowStepId { get; set; }
    public string Query { get; set; }
    public string FlowStepType { get; set; }
    public string CriteriaAnswer { get; set; }
    public List<long> CriteriaSelectedAnswers { get; set; }
}