using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using WebApplication3.Model;

[Authorize]
public class IndexModel : PageModel
{
    private readonly UserManager<ApplicationUser> userManager;
    private readonly IOptions<SessionOptions> sessionOptions;
    private readonly IDataProtectionProvider dataProtectionProvider;

    public IndexModel(
        UserManager<ApplicationUser> userManager,
        IOptions<SessionOptions> sessionOptions,
        IDataProtectionProvider dataProtectionProvider)

    {
        this.userManager = userManager;
        this.sessionOptions = sessionOptions;
        this.dataProtectionProvider = dataProtectionProvider;
    }

    public ApplicationUser UserInfo { get; set; }

    public async Task OnGet()
    {
        var user = await userManager.GetUserAsync(User);

        // Store user information in the session
        HttpContext.Session.SetString("UserId", user.Id);
        HttpContext.Session.SetString("FirstName", user.FirstName);
        HttpContext.Session.SetString("LastName", user.LastName);

        UserInfo = user;

        // Decrypt user's data
        var encryptedFirstName = user.FirstName;
        var unprotectedFirstName = UnprotectData(encryptedFirstName);
        ViewData["DecryptedFirstName"] = unprotectedFirstName;
    }

    // Method to decrypt data
    private string UnprotectData(string protectedData)
    {
        var protector = dataProtectionProvider.CreateProtector("YourPurpose");
        return protector.Unprotect(protectedData);
    }
}
