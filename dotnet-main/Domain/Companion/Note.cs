using System.ComponentModel.DataAnnotations;
using Pip.Domain.Flow;

namespace PIP.Domain.Companion;

public class Note
{
    [Key] public long NoteId { get; set; }

    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime TimeOfCreation { get; set; }
    public FlowSession FlowSession { get; set; }
    public long FlowStepId { get; set; }
}