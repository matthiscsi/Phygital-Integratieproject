using PIP.Domain.Involvement;

namespace PIP.Domain.User;

public class Participant : Role
{
    public int SpentTime { get; set; }

    public Participation Participation { get; set; }
}