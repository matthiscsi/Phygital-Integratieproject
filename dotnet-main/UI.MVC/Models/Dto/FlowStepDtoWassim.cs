using System.ComponentModel.DataAnnotations;
using PIP.Domain.Flow;
using PIP.Domain.Flow.Inquiry;
namespace UI.MVC.Models.Dto;

public class FlowStepDtoWassim
{
    public long FlowStepId { get; set; }

    public string FlowStepType { get; set; }

    public int Ordernr { get; set; }


    [StringLength(30, ErrorMessage = "Header mag niet langer zijn dan 30 tekens.")]
    public string Header { get; set; }


    [StringLength(200, ErrorMessage = "Query mag niet langer zijn dan 200 tekens.")]
    public string Query { get; set; }


    public string Type { get; set; }

    [Required(ErrorMessage = "SubthemeId is verplicht.")]
    public long SubthemeId { get; set; }

    [StringLength(25, ErrorMessage = "SubthemeTitle mag niet langer zijn dan 25 tekens.")]
    public string SubthemeTitle { get; set; }

    [Range(0, 100, ErrorMessage = "MinValue moet tussen 0 en 100 liggen.")]
    public int MinValue { get; set; }

    [Range(0, 100, ErrorMessage = "MaxValue moet tussen 0 en 100 liggen.")]
    public int MaxValue { get; set; }

    [StringLength(200, ErrorMessage = "Body mag niet langer zijn dan 200 tekens.")]
    public string Body { get; set; }

    public ICollection<Option> Options { get; set; }

    public string UploadedImage { get; set; }

    public string UploadedVideo { get; set; }

    public string UploadedAudio { get; set; }
    
    public string UrlImage { get; set; }

    public string UrlVideo { get; set; }

    public string UrlAudio { get; set; }


    public IFormFile Image { get; set; }

    public IFormFile Video { get; set; }

    public IFormFile Audio { get; set; }

    public int Flow { get; set; }

    public FlowStep FlowStep { get; set; }


    public bool IsActive { get; set; }

    public IEnumerable<Subtheme> OtherSubthemes;
    public IEnumerable<ConditionalPointDto> ConditionalPointDtos = new List<ConditionalPointDto>();
}