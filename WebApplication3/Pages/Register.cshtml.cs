using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication3.Model;
using WebApplication3.ViewModels;

namespace WebApplication3.Pages
{
	public class RegisterModel : PageModel
	{
		// Dependency injection for UserManager and SignInManager
		private UserManager<ApplicationUser> userManager { get; }
		private SignInManager<ApplicationUser> signInManager { get; }

		// Property for binding registration data from the form
		[BindProperty]
		public Register RModel { get; set; }

		// Constructor to initialize UserManager and SignInManager
		public RegisterModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
		{
			this.userManager = userManager;
			this.signInManager = signInManager;
		}

		// Handler for HTTP GET request
		public void OnGet() { }

		// Handler for HTTP POST request when registering a new user
		public async Task<IActionResult> OnPostAsync()
		{
			if (ModelState.IsValid)
			{
				// Check if the email already exists
				var existingUser = await userManager.FindByEmailAsync(RModel.Email);

				if (existingUser != null)
				{
					// If email exists, add error to ModelState
					ModelState.AddModelError("RModel.Email", "Email address is already registered.");
					return Page();
				}

				// Create a new user object
				var user = new ApplicationUser()
				{
					// CAN USE PROTECT AND UNPROTECT FOR ENCRYPTION DECRYPTION FROM PRACRICAL
					// AES CREATE A NEW FIELD TO STORE CREDIT CARD ENCRYPTION KEY
					UserName = RModel.Email,
					FirstName = Encryption(RModel.FirstName),
					LastName = RModel.LastName,
					CreditCard = Encryption(RModel.CreditCard),
					PhoneNumber = RModel.PhoneNumber,
					BillingAddress = RModel.BillingAddress,
					ShippingAddress = RModel.ShippingAddress,
					Email = RModel.Email,
				};

				// Password is hashed internally when a user is created
				var result = await userManager.CreateAsync(user, RModel.Password);

				if (result.Succeeded)
				{
					await signInManager.SignInAsync(user, false);
					return RedirectToPage("Index");
				}

				foreach (var error in result.Errors)
				{
					ModelState.AddModelError("", error.Description);
				}
			}

			return Page();
		}

		// Helper method for encrypting sensitive data
		private string Encryption(string data)
		{
			// Use a secure method to generate key and IV in production
			string Key = KeyGenerator.GenerateRandomKey(256); // 256 bits key size
			string IV = KeyGenerator.GenerateRandomIV(128);   // 128 bits block size for AES

			// Encrypt the data using AES encryption
			return AESEncryptionHelper.Encrypt(data, Key, IV);
		}
	}
}
