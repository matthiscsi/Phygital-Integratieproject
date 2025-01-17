using System.ComponentModel.DataAnnotations;

namespace PIP.Domain.User;

public class WebappUser
{
    public long WebappUserId { get; set; }

    [Required(ErrorMessage = "Naam is verplicht")]
    [StringLength(25, ErrorMessage = "Naam mag niet langer zijn dan 25 tekens")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Email is verplicht")]
    [EmailAddress(ErrorMessage = "Ongeldig e-mailadres")]
    public string Email { get; set; }

    public Flow.Flow Flow;
}