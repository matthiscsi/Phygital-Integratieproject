using System.ComponentModel.DataAnnotations;
using PIP.BL.CustomValidationAttributes;


namespace UI.MVC.Models.Dto
{
    public class CompanionDto
    {
        [Required(ErrorMessage = "Email is verplicht.")]
        [StringLength(100, ErrorMessage = "Email cannot be longer than 100 characters.")]
        public string Name { get; set; }


        [CustomPassword(ErrorMessage =
            "Wachtwoord moet minstens 8 tekens lang zijn en minstens 1 hoofdletter, 1 kleine letter, 1 cijfer en 1 speciaal teken bevatten")]

        public string Password { get; set; }
    }
}