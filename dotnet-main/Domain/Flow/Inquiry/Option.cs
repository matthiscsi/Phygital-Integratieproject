using System.ComponentModel.DataAnnotations;

namespace PIP.Domain.Flow.Inquiry;

public class Option
{
    public long Id { get; set; }

    [Required(ErrorMessage = "Tekst bij de opties is verplicht.")]
    public string Text { get; set; }

    public Question Question { get; set; }
}