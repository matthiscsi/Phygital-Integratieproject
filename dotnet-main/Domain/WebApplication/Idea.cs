#nullable enable
using System.ComponentModel.DataAnnotations;

namespace PIP.Domain.WebApplication;

public class Idea
{
    [Key] 
    public long IdeaId { get; set; }
    [Required(ErrorMessage = "Titel is een vereiste.")]
    [StringLength(50, ErrorMessage = "Titel mag niet langer zijn dan 50 tekens.")]
    public string Title { get; set; }

    [Required(ErrorMessage = "Beschrijving is een vereiste.")]
    [StringLength(500, ErrorMessage = "Beschrijving mag niet langer zijn dan 500 tekens.")]
    public string Description { get; set; }
    
    [StringLength(100, ErrorMessage = "link kan niet langer zijn dan 100 tekens.")]
    public string? UrlPhoto { get; set; }
    public IEnumerable<Reaction> reactions { get; set; }

//		public WebappUser WebappUser { get; set; }
}