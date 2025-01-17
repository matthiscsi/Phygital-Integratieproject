using System.ComponentModel.DataAnnotations;

namespace UI.MVC.Views.Models;

public class CreateFlowViewModel : IValidatableObject
{
    [Required]
    public long ProjectId { get; set; }
    [StringLength(30)]
    public string Title { get; set; }
    [StringLength(300)]
    public string Description { get; set; }
    
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        throw new NotImplementedException();
    }
}