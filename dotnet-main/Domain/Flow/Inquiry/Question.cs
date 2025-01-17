using System.ComponentModel.DataAnnotations;

namespace PIP.Domain.Flow.Inquiry {
	public abstract class Question : FlowStep  {
		[Required (ErrorMessage = "Vraag is verplicht.")]
		public string Query { get; set; }
		public ICollection<ConditionalPoint> QuestionConditionalPoints { get; set; } = new List<ConditionalPoint>();
	}

}
