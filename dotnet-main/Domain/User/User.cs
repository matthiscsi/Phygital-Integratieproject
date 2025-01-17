using System.ComponentModel.DataAnnotations;

namespace PIP.Domain.User;

public class User
{
    [Key] public long UserId { get; set; }
    [Required(ErrorMessage = "Naam is verplicht")]
    public string Name { get; set; }
    [Required(ErrorMessage = "Email is verplicht")]
    public string Email { get; set; }
    [Required(ErrorMessage = "Wachtwoord is verplicht")]
    public string Password { get; set; }
    public IEnumerable<Role> Roles { get; set; }
}