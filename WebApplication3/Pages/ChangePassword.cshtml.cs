using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication3.Model;
using WebApplication3.ViewModels;

namespace WebApplication3.Pages
{
    public class ChangePasswordModel : PageModel
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly AuthDbContext dbContext;

        public ChangePasswordModel(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
            AuthDbContext dbContext)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.dbContext = dbContext;
        }

        [BindProperty]
        public ChangePassword ChangePasswordViewModel { get; set; }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{userManager.GetUserId(User)}'.");
            }

            // Check minimum password age
            if (user.LastPasswordChangeDate.HasValue)
            {
                var minPasswordAge = TimeSpan.FromMinutes(30); // Change this to your desired minimum password age
                var timeSinceLastChange = DateTime.UtcNow - user.LastPasswordChangeDate.Value;

                if (timeSinceLastChange < minPasswordAge)
                {
                    ModelState.AddModelError("", $"Cannot change password within {minPasswordAge.TotalMinutes} minutes of the last change.");
                    return Page();
                }
            }

            // Check maximum password age
            if (user.LastPasswordChangeDate.HasValue)
            {
                var maxPasswordAge = TimeSpan.FromDays(90); // Change this to your desired maximum password age
                var timeSinceLastChange = DateTime.UtcNow - user.LastPasswordChangeDate.Value;

                if (timeSinceLastChange > maxPasswordAge)
                {
                    ModelState.AddModelError("", $"Must change password within {maxPasswordAge.TotalDays} days.");
                    return Page();
                }
            }

            // Retrieve the user's password history (e.g., last two hashed passwords)
            var passwordHistory = dbContext.PasswordHistory
                .Where(ph => ph.UserId == user.Id)
                .OrderByDescending(ph => ph.DateChanged)
                .Take(2) // Adjust this to the number of passwords you want to check against
                .ToList();

            // Hash the new password
            var hashedNewPassword = userManager.PasswordHasher.HashPassword(user, ChangePasswordViewModel.NewPassword);
            System.Diagnostics.Debug.WriteLine($"Hashed Password during Password Change: {hashedNewPassword}");

            // Check if the new password matches any of the hashed passwords in the user's password history
            if (passwordHistory.Any(ph => userManager.PasswordHasher.VerifyHashedPassword(user, ph.PasswordHash, ChangePasswordViewModel.NewPassword) == PasswordVerificationResult.Success))
            {
                ModelState.AddModelError("", "Cannot reuse the same password. Choose a different one.");
                return Page();
            }

            // Attempt to change the user's password
            var changePasswordResult = await userManager.ChangePasswordAsync(user, ChangePasswordViewModel.CurrentPassword, ChangePasswordViewModel.NewPassword);

            if (!changePasswordResult.Succeeded)
            {
                // If password change fails, add errors to model state
                foreach (var error in changePasswordResult.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return Page();
            }

            System.Diagnostics.Debug.WriteLine("Password changed successfully.");

            // Save the new password to the password history
            dbContext.PasswordHistory.Add(new PasswordHistory
            {
                UserId = user.Id,
                PasswordHash = hashedNewPassword,
                DateChanged = DateTime.UtcNow
            });

            // Remove old password history entries if necessary
            if (passwordHistory.Count >= 2)
            {
                dbContext.PasswordHistory.RemoveRange(passwordHistory.Skip(1));
            }

            // Update the LastPasswordChangeDate property
            user.LastPasswordChangeDate = DateTime.UtcNow;
            await userManager.UpdateAsync(user);

            await dbContext.SaveChangesAsync();

            // Refresh the user's sign-in cookie
            await signInManager.RefreshSignInAsync(user);
            return RedirectToPage("/Index");
        }
    }
}