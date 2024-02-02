using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication3.Model;
using WebApplication3.ViewModels;

namespace WebApplication3.Pages
{
    public class ResetPasswordModel : PageModel
    {
        private readonly UserManager<ApplicationUser> userManager;

        public ResetPasswordModel(
            UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }

        [BindProperty]
        public ResetPassword ResetPasswordViewModel { get; set; }

        public IActionResult OnGet(string userId, string code)
        {
            // Validate the userId and code
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(code))
            {
                return BadRequest("Invalid user ID or code");
            }

            ResetPasswordViewModel = new ResetPassword
            {
                UserId = userId,
                Code = code
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await userManager.FindByIdAsync(ResetPasswordViewModel.UserId);

            if (user == null)
            {
                return Page();
            }

            var result = await userManager.ResetPasswordAsync(user, ResetPasswordViewModel.Code, ResetPasswordViewModel.Password);

            if (result.Succeeded)
            {
                return RedirectToPage("Login");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return Page();
            }
        }
    }
}
