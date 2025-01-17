using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using PIP.Domain.Deelplatform;

namespace PIP.Domain.User {
	public class SubPlatformAdministrator :IdentityUser  {
		
		[Required(ErrorMessage = "Organisatie naam is verplicht")]
		public string OrganizationName { get; set; }
		public Subplatform Subplatform { get; set; }
	}

}
