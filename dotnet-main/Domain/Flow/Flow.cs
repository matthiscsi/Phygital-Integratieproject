#nullable enable
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PIP.Domain.Deelplatform;
using Pip.Domain.Flow;

namespace PIP.Domain.Flow;

public class Flow
{
    public Flow(string title, Project project, FlowStrategy flowStrategy, Theme theme)
    {
        Title = title;
        Project = project;
        FlowStrategy = flowStrategy;
        Theme = theme;
        FlowSteps = new List<FlowStep>();
    }
    public Flow(){}

    [Key] public long FlowId { get; set; }
    
    [Required(ErrorMessage = "Title is required")]
    public string Title { get; set; }

    [Required(ErrorMessage = "Description is required")]
    public string? Description { get; set; }

    public string? UrlFlowPicture { get; set; }
    
    public IEnumerable<FlowStep> FlowSteps { get; set; }
    public Project Project { get; set; }

    [NotMapped] public FlowStrategy FlowStrategy { get; set; }


    public Theme Theme { get; set; }
    
    
    
    public ICollection<FlowSession>? FlowSessions { get; set; }
    public bool CirculaireFlow { get; set; }
    public bool Physical { get; set; }


}