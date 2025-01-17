using System.ComponentModel.DataAnnotations;
using PIP.Domain.Flow.Inquiry;
using PIP.Domain.User;

namespace PIP.Domain.Involvement;

public class Participation
{
    [Key] public long ParticipationId { get; set; }

    public DateTime SessionDate { get; set; }

    public IEnumerable<Response> Responses { get; set; }

    public IEnumerable<Participant> Participants { get; set; }
}