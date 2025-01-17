using System.Reflection;
using Microsoft.Extensions.Configuration;
using PIP.BL;

namespace PIP.BL;
public class JsonProfanityFilter : IProfanityFilter
{
    private readonly List<string> _blacklistedWords;

    public JsonProfanityFilter()
    {
        var basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(basePath, "Configuration"))
            .AddJsonFile("ProfanityFilter.json")
            .Build();

        _blacklistedWords = configuration.GetSection("BlacklistedWords")
            .GetChildren()
            .Select(x => x.Value)
            .ToList();
    }

    public bool ContainsProfanity(string text)
    {
        string[] words = text.Split(' ', '.', ',', ';', '!', '?');
        foreach (var word in words)
        {
            if (_blacklistedWords.Contains(word.ToLower()))
            {
                return true;
            }
        }
        return false;
    }

    public bool AddProfanity(string text)
    {
        throw new NotImplementedException();
    }
}