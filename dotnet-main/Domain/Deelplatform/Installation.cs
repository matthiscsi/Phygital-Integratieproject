using System.ComponentModel.DataAnnotations;
using Pip.Domain.Flow;

namespace PIP.Domain.Deelplatform;

public class Installation
{
    [Key] public long InstallationId { get; set; }
    public IEnumerable<FlowSession> Sessions { get; set; }
}