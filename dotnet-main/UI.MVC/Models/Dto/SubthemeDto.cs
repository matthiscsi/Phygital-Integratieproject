using System.ComponentModel.DataAnnotations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Query.Expressions.Internal;

namespace UI.MVC.Models.Dto;

public class SubthemeDto
{
    public long Id { get; set; }

    [Required(ErrorMessage = "Titel is verplicht.")]
    [StringLength(25, ErrorMessage = "Titel mag niet meer dan 25 karakters zijn.")]
    public string Title { get; set; }

    [Required(ErrorMessage = "Body is verplicht.")]
    [StringLength(100, ErrorMessage = "Body mag niet meer dan 100 karakters zijn.")]
    public string Body { get; set; }
    public string? UrlPhoto { get; set; }
    public IFormFile Photo { get; set; }
    public int flow { get; set; }
}