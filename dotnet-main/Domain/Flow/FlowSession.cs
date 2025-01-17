#nullable enable
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using PIP.Domain.Companion;
using PIP.Domain.Flow;
using PIP.Domain.Flow.Inquiry;

namespace Pip.Domain.Flow;

public class FlowSession
{
    public FlowSession(CirculaireFlowStrategy circulaireFlow)
    {
        CirculaireFlows = circulaireFlow;
        SessionStartDate = DateTime.Now;
        State = State.Idle;
    }

    public FlowSession(PIP.Domain.Flow.Flow flow)
    {
        Flow = flow;
        SessionStartDate = DateTime.Now;
        State = State.Idle;
    }

    public FlowSession()
    {
    }

    [Key] public long FlowSessionId { get; set; }
    public PIP.Domain.Flow.Flow? Flow { get; set; }
    public ICollection<Note> Notes { get; set; } = new List<Note>();

    public CirculaireFlowStrategy? CirculaireFlows { get; set; }
    public DateTime SessionStartDate { get; set; }
    public DateTime SessionEndDate { get; set; }

    public State State { get; set; }

    public FlowStep? CurrentFlowStep { get; set; }
    public int? ElapsedTime { get; set; }
    public IEnumerable<Response> Responses { get; set; }
    [NotMapped] public List<long> PassedSubthemes { get; set; } = new List<long>();
    public int ExpectedUsers { get; set; }

    public string PassedSubthemesIdsJson
    {
        get => JsonConvert.SerializeObject(PassedSubthemes);
        set => PassedSubthemes = JsonConvert.DeserializeObject<List<long>>(value);
    }
}