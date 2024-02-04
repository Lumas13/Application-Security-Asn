using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApplication3.Model;
using WebApplication3.ViewModels;

namespace WebApplication3.Pages
{
    public class RegisterModel : PageModel
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IDataProtectionProvider dataProtectionProvider;
        private readonly AuthDbContext dbContext;

        [BindProperty]
        public Register RModel { get; set; }

        public RegisterModel(
            UserManager<ApplicationUser> userManager, 
            SignInManager<ApplicationUser> signInManager,
            IDataProtectionProvider dataProtectionProvider, 
            AuthDbContext dbContext)

        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.dataProtectionProvider = dataProtectionProvider;
            this.dbContext = dbContext;
        }

        public void OnGet() 
        {

        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                // Check if the email is already registered
                var existingUser = await userManager.FindByEmailAsync(RModel.Email);

                if (existingUser != null)
                {
                    ModelState.AddModelError("RModel.Email", "Email address is already registered.");
                    return Page();
                }

                var user = new ApplicationUser()
                {
                    UserName = RModel.Email,
                    FirstName = ProtectData(RModel.FirstName),
                    LastName = RModel.LastName,
                    CreditCard = ProtectData(RModel.CreditCard),
                    PhoneNumber = RModel.PhoneNumber,
                    BillingAddress = RModel.BillingAddress,
                    ShippingAddress = RModel.ShippingAddress,
                    Email = RModel.Email,
                };

                // Handle file upload
                if (RModel.PhotoFile != null && RModel.PhotoFile.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await RModel.PhotoFile.CopyToAsync(memoryStream);
                        user.Photo = memoryStream.ToArray();
                    }
                }

                // Add user into the database, password hashed by CreateAsync
                var result = await userManager.CreateAsync(user, RModel.Password);

                if (result.Succeeded)
                {
                    // Enable Two-Factor Authentication for the user
                    await userManager.SetTwoFactorEnabledAsync(user, true);

                    await signInManager.SignInAsync(user, false);
                    return RedirectToPage("Index");
                }
            }

            return Page();
        }

        // Data Encryption
        private string ProtectData(string data)
        {
            var protector = dataProtectionProvider.CreateProtector("YourPurpose");

            return protector.Protect(data);
        }
    }
}
