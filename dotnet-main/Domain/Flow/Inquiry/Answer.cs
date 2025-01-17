using System.ComponentModel.DataAnnotations;

namespace PIP.Domain.Flow.Inquiry;

public abstract class Answer
{
    [Key] public long AnswerId { get; set; }
    public ConditionalPoint ConditionalPoint { get; set; }
    public long? ConditionalPointId { get; set; }
    public Response Response { get; set; }
    public long? ResponseId { get; set; }

}