
using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Identity;

using PIP.Domain.Companion;
using PIP.Domain.Deelplatform;

namespace PIP.Domain.User;

public class Companion :IdentityUser  {
		

	public Subplatform Subplatform { get; set; }

}