using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace PIP.Domain.Deelplatform;

public class Project
{
    [Key] public long ProjectId { get; set; }
    public ICollection<Flow.Flow> Flows { get; set; }
    public Subplatform Subplatform { get; set; }

    public ICollection<IdentityUser> Companions { get; set; }
    public IdentityUser SubPlatformAdmin { get; set; }
    
    [Required(ErrorMessage = "Actief is verplicht")]
    public bool IsActive { get; set; }
    [Required(ErrorMessage = "Naam is verplicht")]
    public string Name { get; set; }
    [Required(ErrorMessage = "Beschrijving is verplicht")]
    public string Description { get; set; }
    public bool CirculaireFlow { get; set; }
    public string BackgroundColor { get; set; }
    public string Font { get; set; }
}