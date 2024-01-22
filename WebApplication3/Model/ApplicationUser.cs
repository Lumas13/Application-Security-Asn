using Microsoft.AspNetCore.Identity;

namespace WebApplication3.Model
{
	public class ApplicationUser : IdentityUser
	{
		public string? FirstName { get; set; }

		public string? LastName { get; set; }
		
		public string? CreditCard { get; set; }

		public string? BillingAddress { get; set; }

		public string? ShippingAddress { get; set; }

		public byte[]? Photo { get; set; }		

        // MobileNo, Email, Password use built in fields
    }
}
