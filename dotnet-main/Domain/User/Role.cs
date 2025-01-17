using System.ComponentModel.DataAnnotations;

namespace PIP.Domain.User;

public class Role
{
    [Key] public long RoleId { get; set; }
}