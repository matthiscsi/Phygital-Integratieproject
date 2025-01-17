namespace UI.MVC.Models.Dto;

public class NoteDto
{
    public long flowsessionId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public long flowstepId { get; set; }
}