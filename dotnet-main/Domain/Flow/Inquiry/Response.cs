using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Pip.Domain.Flow;

namespace PIP.Domain.Flow.Inquiry;

public class Response
{
    [Key] public long ResponseId { get; set; }
    public Answer Answer { get; set; }
    public long AnswerId { get; set; }
    public Question Question { get; set; }
    public long FlowSessionId { get; set; }

    public FlowSession FlowSession { get; set; }
}