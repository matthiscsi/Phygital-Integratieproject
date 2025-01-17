using System.ComponentModel.DataAnnotations;
using Pip.Domain.Flow;

namespace PIP.Domain.Flow;

public class CirculaireFlowStrategy : FlowStrategy
{
    [Key]public long CirculaireFlowId { get; set; }
    
    

    public ICollection<FlowSession> FlowSessions { get; set; }
    public ICollection<Flow> Flows { get; set; }
}