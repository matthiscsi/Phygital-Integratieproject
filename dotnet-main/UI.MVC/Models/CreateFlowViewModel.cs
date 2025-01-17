using System.ComponentModel.DataAnnotations;
using PIP.Domain.Flow;

namespace UI.MVC.Models;

public class CreateFlowViewModel //: IValidatableObject
{
    [Range(1, long.MaxValue, ErrorMessage = "Selecteer een project.")]
    public long ProjectId { get; set; }

    [Required(ErrorMessage = "Titel is nodig.")]
    [StringLength(25, ErrorMessage = "Titel mag niet langer zijn dan 25 tekens.")]
    public string Title { get; set; }

    [Required(ErrorMessage = "Beschrijving is nodig")]
    [StringLength(100, ErrorMessage = "Beschrijving mag niet langer zijn dan 100 tekens.")]
    public string Description { get; set; }

    [Required(ErrorMessage = "Vul alle thema velden in")]
    public Theme Theme { get; set; }

    [Required(ErrorMessage = "Geef een foto aan de flow.")]
    public IFormFile UploadedThemePicture { get; set; }
    public bool Physical { get; set; }
}