using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using WebApplication3.Model;

namespace WebApplication3.Pages
{
	public class LogoutModel : PageModel
	{
		private readonly SignInManager<ApplicationUser> signInManager;

		public LogoutModel(SignInManager<ApplicationUser> signInManager)
		{
			this.signInManager = signInManager;
		}

		public void OnGet()
		{

		}

		public async Task<IActionResult> OnPostLogoutAsync()
		{
			// Sign the user out
			await signInManager.SignOutAsync();

			// Clear the session data
			HttpContext.Session.Clear();

			// Redirect to the login page
			return RedirectToPage("Login");
		}

		public async Task<IActionResult> OnPostDontLogoutAsync()
		{
			// Redirect to the index page without logging out
			return RedirectToPage("Index");
		}
	}
}
