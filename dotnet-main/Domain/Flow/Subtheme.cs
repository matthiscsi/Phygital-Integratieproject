using System.ComponentModel.DataAnnotations;

namespace PIP.Domain.Flow;

public class Subtheme
{
    [Key] public long SubthemeId { get; set; }
    
    [Required(ErrorMessage = "Titel is verplicht.")]
    [StringLength(50, ErrorMessage = "Titel mag niet meer dan 50 karakters zijn.")]
    public string Title { get; set; }

    [Required(ErrorMessage = "Body is verplicht.")]
    [StringLength(500, ErrorMessage = "Body mag niet meer dan 100 karakters zijn.")]
    public string Body { get; set; }
    
    public string? UrlPhoto { get; set; }

    public Theme ParentTheme { get; set; }

    public override bool Equals(object obj)
    {
        if (obj == null || !(obj is Subtheme)) return false;

        return SubthemeId == ((Subtheme)obj).SubthemeId;
    }
}