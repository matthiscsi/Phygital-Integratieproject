using System.ComponentModel.DataAnnotations;

namespace UI.MVC.Models.Dto
{
    public class SubPlatformAdministratorDto
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "Naam is vereist")]
        [StringLength(25, ErrorMessage = "Naam mag niet langer zijn dan 25 tekens")]
        public string Name { get; set; }

        [Required(ErrorMessage = "E-mail is vereist")]
        [EmailAddress(ErrorMessage = "Ongeldig e-mailadres")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Link is vereist")]
        [Url(ErrorMessage = "Ongeldige URL")]
        public string Link { get; set; }
        [StringLength(100, ErrorMessage = "Naam mag niet langer zijn dan 100 tekens")]
        [Required(ErrorMessage = "Introductie organisatie is vereist")]
        public string MainText { get; set; }

        public long SubPlatformId { get; set; }


    }
}