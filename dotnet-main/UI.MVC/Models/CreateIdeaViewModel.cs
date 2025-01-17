using System.ComponentModel.DataAnnotations;

namespace UI.MVC.Models;

public class CreateIdeaViewModel
{
    public long FlowId { get; set; }

    [Required(ErrorMessage = "Title is required")]
    [StringLength(100, ErrorMessage = "Title may not be longer than 100 characters")]
    public string Title { get; set; }

    [Required(ErrorMessage = "Description is required")]
    public string Description { get; set; }

    public IFormFile Photo { get; set; }
}