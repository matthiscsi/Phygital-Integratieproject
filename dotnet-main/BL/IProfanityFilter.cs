namespace PIP.BL;

public interface IProfanityFilter
{
    public bool ContainsProfanity(string text);
    public bool AddProfanity(string text);
}