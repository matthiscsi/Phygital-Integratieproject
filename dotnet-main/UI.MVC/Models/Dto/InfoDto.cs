using System.ComponentModel.DataAnnotations;
using IP.Models.Dto;
using PIP.Domain.Flow;

namespace UI.MVC.Models.Dto;

public class InfoDto : FlowStepDto
{
   [Required(ErrorMessage = "Body is verplicht.")]
   public string Body { get; set; }
   public string UrlImage { get; set; }
   public string UrlVideo { get; set; }
   public string UrlAudio { get; set; }
   public string Header { get; set; }
}