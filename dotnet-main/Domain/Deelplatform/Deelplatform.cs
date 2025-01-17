using System.ComponentModel.DataAnnotations;

namespace PIP.Domain.Deelplatform;

public class Subplatform
{
    [Key] public long SubplatformId { get; set; }

    [Required(ErrorMessage = "Naam is verplicht")]
    public string CustomerName { get; set; }
    [Required(ErrorMessage = "Email is verplicht")]
    public string Email { get; set; }
    [Required(ErrorMessage = "Main text is verplicht")]
    public string MainText { get; set; }
    
    public string Link { get; set; }
    public ICollection<Project> Projects { get; set; }
}