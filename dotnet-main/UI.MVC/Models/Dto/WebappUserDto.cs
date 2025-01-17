using System.ComponentModel.DataAnnotations;

namespace UI.MVC.Models.Dto;

public class WebappUserDto
{
    [Required(ErrorMessage = "Naam is verplicht")]
    [StringLength(100, ErrorMessage = "Naam mag niet langer zijn dan 100 tekens")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Email is verplicht")]
    [EmailAddress(ErrorMessage = "Ongeldig e-mailadres")]
    public string Email { get; set; }


    public long FlowId { get; set; }
}