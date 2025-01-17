using System.ComponentModel.DataAnnotations;

namespace UI.MVC.Models.Dto;

public class ProjectDto
{
    [Required(ErrorMessage = "Naam is verplicht.")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Beschrijving is verplicht.")]
    public string Description { get; set; }

    public long ProjectId { get; set; }

    public bool Circulair { get; set; }
    [Required(ErrorMessage = "Selecteer of het actief moet zijn of niet.")]
    public bool isActive { get; set; }

    public string BackgroundColor { get; set; }

    public string Font { get; set; }
}