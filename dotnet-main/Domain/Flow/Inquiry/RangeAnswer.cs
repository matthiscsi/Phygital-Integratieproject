using System.ComponentModel.DataAnnotations;

namespace PIP.Domain.Flow.Inquiry;

public class RangeAnswer : Answer
{
    [Range(0, int.MaxValue)] public long SelectedValue { get; set; }
}