using System.ComponentModel.DataAnnotations;
using PIP.BL.CustomValidationAttributes;
using PIP.Domain.Deelplatform;

namespace UI.MVC.Models.Dto;

public class CreateSubPlatformAdminViewModel
{
    [Required(ErrorMessage = "Naam is vereist")]
    [StringLength(100, ErrorMessage = "Naam mag niet langer zijn dan 100 tekens")]
    public string Name { get; set; }

    [Required(ErrorMessage = "E-mail is vereist")]
    [EmailAddress(ErrorMessage = "Ongeldig e-mailadres")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Wachtwoord is vereist")]
    [CustomPassword(ErrorMessage =
        "Wachtwoord moet minstens 8 tekens lang zijn en minstens 1 hoofdletter, 1 kleine letter, 1 cijfer en 1 speciaal teken bevatten")]
    public string Password { get; set; }

    [Required(ErrorMessage = "Hoofdtekst is vereist")]
    public string MainText { get; set; }

    [Required(ErrorMessage = "Link is vereist")]
    [Url(ErrorMessage = "Ongeldige URL")]
    public string Link { get; set; }

    public IEnumerable<Project> Projects { get; set; }
}