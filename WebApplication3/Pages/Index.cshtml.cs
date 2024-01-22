using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using WebApplication3.Model;

[Authorize]
public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;
    private readonly UserManager<ApplicationUser> userManager;
    private readonly IOptions<SessionOptions> sessionOptions;

    public IndexModel(
       ILogger<IndexModel> logger, UserManager<ApplicationUser> userManager, IOptions<SessionOptions> sessionOptions)
    {
        _logger = logger;
        this.userManager = userManager;
        this.sessionOptions = sessionOptions;
    }

    public ApplicationUser UserInfo { get; set; }

    public void OnGet()
    {
        // Get the currently logged-in user
        var user = userManager.GetUserAsync(User).Result;

        // Store user information in the session
        HttpContext.Session.SetString("UserId", user.Id);
        HttpContext.Session.SetString("FirstName", user.FirstName);
        HttpContext.Session.SetString("LastName", user.LastName);

        // Populate the UserInfo property with user information
        UserInfo = user;

    }
}
