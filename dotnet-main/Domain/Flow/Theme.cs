using System.ComponentModel.DataAnnotations;
using PIP.Domain.WebApplication;

namespace PIP.Domain.Flow;

public class Theme
{
    [Key] public long ThemeId { get; set; }
    [Required(ErrorMessage = "De thema titel is verplicht")]
    public string Title { get; set; }
    [Required(ErrorMessage = "De thema beschrijving is verplicht")]
    public string Body { get; set; }
    public string? UrlThemePicture { get; set; }
    public IEnumerable<Idea> Ideas { get; set; }
    public Flow Flow { get; set; }

    public IEnumerable<Subtheme> SubThemes { get; set; }
}