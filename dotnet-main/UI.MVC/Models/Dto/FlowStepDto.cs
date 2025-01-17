namespace UI.MVC.Models.Dto;

public class FlowStepDto
{
   public long FlowStepId { get; set; }
   public string FlowStepType { get; set; }
   public int OrderNr { get; set; }
   public string Query { get; set; }
   public bool IsActive { get; set; }
   public string Subtheme { get; set; }
   public long SubthemeId { get; set; }
   public long FlowId { get; set; }
}