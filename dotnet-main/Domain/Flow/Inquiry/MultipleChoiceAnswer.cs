using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace PIP.Domain.Flow.Inquiry;

public class MultipleChoiceAnswer : Answer
{
    [NotMapped] public List<long> SelectedAnswers { get; set; }

    [Column("SelectedAnswers")]
    public string SelectedAnswersJson
    {
        get => JsonConvert.SerializeObject(SelectedAnswers);
        set => SelectedAnswers = JsonConvert.DeserializeObject<List<long>>(value);
    }
}