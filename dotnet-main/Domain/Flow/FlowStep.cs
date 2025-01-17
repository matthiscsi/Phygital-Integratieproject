using System.ComponentModel.DataAnnotations;
using PIP.Domain.Flow.Inquiry;

namespace PIP.Domain.Flow
{
    public abstract class FlowStep
    {
        [Key] public long FlowStepId { get; set; }
        public int OrderNr { get; set; }
        public string Header { get; set; }
        public bool IsActive { get; set; }
        public Subtheme SubTheme { get; set; }
        public Flow Flow { get; set; }
        public long FlowId { get; set; }
        public long? ConditionalPointId { get; set; }

        public ConditionalPoint ConditionalPoint { get; set; }
    }
}