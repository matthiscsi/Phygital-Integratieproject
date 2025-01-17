using System.ComponentModel.DataAnnotations;

namespace PIP.Domain.Flow;

public class Info : FlowStep
{
    [MinLength(2, ErrorMessage = "Inhoud moet ingevuld worden.")]
    public string Body { get; set; }
    public string UrlImage { get; set; }
    public string UrlVideo { get; set; }
    public string UrlAudio { get; set; }
}