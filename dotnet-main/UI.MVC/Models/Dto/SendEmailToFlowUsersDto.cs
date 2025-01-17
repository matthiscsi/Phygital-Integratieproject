using System.ComponentModel.DataAnnotations;

namespace UI.MVC.Models.Dto;

public class SendEmailToFlowUsersDto
{

    public int FlowId { get; set; }

    [Required(ErrorMessage = "Onderwerp is vereist.")]
    [StringLength(100, ErrorMessage = "Onderwerp mag niet langer zijn dan 100 tekens.")]
    public string Subject { get; set; }

    [Required(ErrorMessage = "Bericht is vereist.")]
    [StringLength(500, ErrorMessage = "Bericht mag niet langer zijn dan 500 tekens.")]
    public string MessageToUsers { get; set; }
}