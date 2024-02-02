using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using WebApplication3.Model;
using WebApplication3.ViewModels;
using WebApplication3.Helper;
using Newtonsoft.Json;

namespace WebApplication3.Pages
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public Login LModel { get; set; }

        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IOptions<IdentityOptions> identityOptions;
        private readonly EmailService emailService;

        public LoginModel(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            IOptions<IdentityOptions> identityOptions,
            EmailService emailService)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.identityOptions = identityOptions;
            this.emailService = emailService;
        }

        
        public void OnGet()
        {
            
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Extract reCAPTCHA response from the form
            var recaptchaResponse = HttpContext.Request.Form["g-recaptcha-response"];
            var recaptchaSecretKey = "6LeqklkpAAAAAAmFbCoYet1iyxsLpOCtcI4c3z7R";

            // Use HttpClient to verify reCAPTCHA response with Google's API
            using (var recaptchaClient = new HttpClient())
            {
                // Make a request to Google reCAPTCHA API for verification
                var recaptchaResult = await recaptchaClient.GetStringAsync($"https://www.google.com/recaptcha/api/siteverify?secret={recaptchaSecretKey}&response={recaptchaResponse}");

                // Deserialize the response from reCAPTCHA API
                var recaptchaData = JsonConvert.DeserializeObject<RecaptchaResponse>(recaptchaResult);

                // Check if reCAPTCHA verification was successful
                if (!recaptchaData.Success)
                {
                    ModelState.AddModelError(string.Empty, "reCAPTCHA verification failed. Please try again.");
                    return Page();
                }
            }

            var user = await userManager.FindByEmailAsync(LModel.Email);

            if (ModelState.IsValid)
            {
                var result = await signInManager.CheckPasswordSignInAsync(user, LModel.Password, lockoutOnFailure: false);

                if (result.IsLockedOut)
                {
                    ModelState.AddModelError("LModel.Email", "Account is locked out. Please try again later.");
                    return Page();
                }
                else if (result.Succeeded)
                {
                    // 2FA
                    if (user != null && user.TwoFactorEnabled)
                    {
                        // Generate a 2FA token using UserManager
                        var token = await userManager.GenerateTwoFactorTokenAsync(user, "Email");

                        System.Diagnostics.Debug.WriteLine($"Generated 2FA token for {LModel.Email}: {token}");

                        await emailService.SendTwoFactorCodeAsync(LModel.Email, token);

                        return RedirectToPage("TwoFactorAuth", new { email = LModel.Email });
                    }
                }
                else
                {
                    // Account Lockout
                    if (user != null)
                    {
                        user.AccessFailedCount++;

                        if (user.AccessFailedCount >= userManager.Options.Lockout.MaxFailedAccessAttempts)
                        {
                            user.LockoutEnd = DateTimeOffset.UtcNow.Add(userManager.Options.Lockout.DefaultLockoutTimeSpan);
                        }

                        await userManager.UpdateAsync(user);
                    }

                    ModelState.AddModelError("LModel.Email", "Email or password is incorrect.");
                }
            }

            return Page();
        }
    }
}
