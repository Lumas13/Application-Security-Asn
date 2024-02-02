using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication3.Helper;
using WebApplication3.Model;
using WebApplication3.ViewModels;

namespace WebApplication3.Pages
{
    public class ForgetPasswordModel : PageModel
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly EmailService emailService;

        public ForgetPasswordModel(
            UserManager<ApplicationUser> userManager, 
            EmailService emailService)
        {
            this.userManager = userManager;
            this.emailService = emailService;
        }

        [BindProperty]
        public ForgetPassword ForgetPasswordViewModel { get; set; }

        public void OnGet()
        {
            
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await userManager.FindByEmailAsync(ForgetPasswordViewModel.Email);

            if (user == null)
            {
                return Page();
            }

            // Generate and send the reset link
            var resetToken = await userManager.GeneratePasswordResetTokenAsync(user);

            // Create callback URL to reset password
            var callbackUrl = Url.Page(
                "/ResetPassword",
                pageHandler: null,
                values: new { area = "Identity", userId = user.Id, code = resetToken },
                protocol: Request.Scheme);

            // Send the reset link to the user's email
            await emailService.SendResetLinkAsync(ForgetPasswordViewModel.Email, callbackUrl);

            // Redirect to a confirmation page
            return RedirectToPage("Login");
        }
    }
}
