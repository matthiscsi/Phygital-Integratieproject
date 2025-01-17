namespace PIP.Domain.Flow.Inquiry;

public class ConditionalPoint
{
    public long ConditionalPointId { get; set; }
    public Question Question { get; set; }
    public FlowStep FollowUpStep { get; set; }
    public Answer Criteria { get; set; }

    public ConditionalPoint()
    {
    }

    public ConditionalPoint(Answer answer, OpenQuestion followUpStep)
    {
        Criteria = answer;
        FollowUpStep = followUpStep;
    }
}