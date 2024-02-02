using Microsoft.AspNetCore.Identity;
using WebApplication3.Helper;
using WebApplication3.Model;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();

builder.Services.AddDbContext<AuthDbContext>();

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // Account Limiting
    options.Lockout.AllowedForNewUsers = true;
    options.Lockout.MaxFailedAccessAttempts = 3; // Number of allowed failed attempts
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5); // Lockout duration
})
.AddEntityFrameworkStores<AuthDbContext>()
.AddDefaultTokenProviders();

// Configure Application Cookie
builder.Services.ConfigureApplicationCookie(config =>
{
    config.LoginPath = "/Login";
});

// Session Management
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddDistributedMemoryCache(); // Save session in memory
builder.Services.AddSession(options =>
{
    // Session Timeout
    options.IdleTimeout = TimeSpan.FromSeconds(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Audit Log
builder.Services.AddScoped<AuditLogHelper>();

// Email Service
builder.Services.AddTransient<EmailService>();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseStatusCodePagesWithRedirects("/errors/404");
app.UseExceptionHandler("/errors/500");

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.UseSession();

app.MapRazorPages();

app.Run();
