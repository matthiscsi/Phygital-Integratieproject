using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PIP.Domain.Flow.Inquiry
{
    public class RangeQuestion : Question, IValidatableObject
    {
        [Required(ErrorMessage = "Minimum waarde is verplicht.")]
        public int MinValue { get; set; }

        [Required(ErrorMessage = "Maximum waarde is verplicht.")]
        [Range(2, int.MaxValue, ErrorMessage = "Maximum waarde moet groter zijn dan 1.")]
        public int MaxValue { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (MinValue >= MaxValue)
            {
                yield return new ValidationResult(
                    "Minimum waarde moet kleiner zijn dan Maximum waarde.",
                    new[] { nameof(MinValue), nameof(MaxValue) });
            }
        }
    }
}