using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using System;
using System.ComponentModel.DataAnnotations;
using WebApplication3.Model;
using WebApplication3.ViewModels;
using WebApplication3.Helper;

namespace WebApplication3.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public Login LModel { get; set; }

        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IOptions<IdentityOptions> identityOptions;
        private readonly AuditLogHelper auditLogHelper;

        public LoginModel(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager,
            IOptions<IdentityOptions> identityOptions, AuditLogHelper auditLogHelper)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.identityOptions = identityOptions;
            this.auditLogHelper = auditLogHelper;
        }

        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Validate email format
            if (!IsValidEmail(LModel.Email))
            {
                ModelState.AddModelError("LModel.Email", "Invalid email format.");
            }

            // Validate password length
            if (LModel.Password.Length < 12)
            {
                ModelState.AddModelError("LModel.Password", "Password must be at least 12 characters long.");
            }

            // Validate other input requirements as needed

            if (ModelState.IsValid)
            {
                // Attempt to sign in the user using the provided credentials
                var result = await signInManager.PasswordSignInAsync(LModel.Email, LModel.Password, LModel.RememberMe, false);

                if (result.Succeeded)
                {
                    // Use AuditLogHelper to log user login
                    await auditLogHelper.LogUserLoginAsync(LModel.Email);

                    return RedirectToPage("Index");
                }
                else if (result.IsLockedOut)
                {
                    ModelState.AddModelError("LModel.Email", "Account is locked out. Please try again later.");
                }
                else if (!result.Succeeded)
                {
                    // Update AccessFailedCount and LockoutEnd if necessary for unsuccessful login attempts
                    var user = await userManager.FindByEmailAsync(LModel.Email);

                    if (user != null)
                    {
                        // Increment the count of failed access attempts
                        user.AccessFailedCount++;

                        // Check if the maximum allowed failed attempts have been reached
                        if (user.AccessFailedCount >= userManager.Options.Lockout.MaxFailedAccessAttempts)
                        {
                            // Set the lockout end date to the current time plus the lockout duration
                            user.LockoutEnd = DateTimeOffset.UtcNow.Add(userManager.Options.Lockout.DefaultLockoutTimeSpan);
                        }

                        // Update the user in the database with the modified properties
                        await userManager.UpdateAsync(user);
                    }

                    ModelState.AddModelError("LModel.Email", "Username or Password incorrect");
                }
            }

            return Page();
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var mailAddress = new System.Net.Mail.MailAddress(email);
                return mailAddress.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
