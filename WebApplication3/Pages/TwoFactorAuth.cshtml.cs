using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using WebApplication3.Helper;
using WebApplication3.Model;

namespace WebApplication3.Pages
{
    public class TwoFactorAuthModel : PageModel
    {
        [BindProperty]
        public string TwoFactorCode { get; set; }

        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly AuditLogHelper auditLogHelper;

        public TwoFactorAuthModel(
            SignInManager<ApplicationUser> signInManager, 
            UserManager<ApplicationUser> userManager, 
            AuditLogHelper auditLogHelper)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.auditLogHelper = auditLogHelper;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Retrieve email from the URL
            var email = HttpContext.Request.Query["email"];

            var user = await userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return NotFound($"Unable to load two-factor authentication user.");
            }

            // Validate the 2FA code
            var result = await userManager.VerifyTwoFactorTokenAsync(user, "Email", TwoFactorCode);

            if (result)
            {
                // Use AuditLogHelper to log user login
                await auditLogHelper.LogUserLoginAsync(user.Email);

                // 2FA code is valid, sign in the user
                await signInManager.SignInAsync(user, false);


                return RedirectToPage("Index");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid 2FA code. Please try again.");

                return Page();
            }
        }
    }
}
