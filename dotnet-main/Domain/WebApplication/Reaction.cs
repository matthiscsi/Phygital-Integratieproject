using System.ComponentModel.DataAnnotations;

namespace PIP.Domain.WebApplication;

public class Reaction
{
    [Key] public long ReactionId { get; set; }

    public Idea Idea { get; set; }
}