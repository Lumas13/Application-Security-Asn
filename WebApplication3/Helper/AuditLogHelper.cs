using WebApplication3.Model;

namespace WebApplication3.Helper
{
	public class AuditLogHelper
	{
		private readonly ILogger<AuditLogHelper> _logger;
		private readonly AuthDbContext _context;

		public AuditLogHelper(ILogger<AuditLogHelper> logger, AuthDbContext context)
		{
			_logger = logger;
			_context = context;
		}

		public async Task LogUserLoginAsync(string userId)
		{
			await LogUserActivityAsync(userId, "User Logged In");
		}

		public async Task LogUserLogoutAsync(string userId)
		{
			await LogUserActivityAsync(userId, "User Logged Out");
		}

		private async Task LogUserActivityAsync(string userId, string action)
		{
			var auditLog = new AuditLog
			{
				UserId = userId,
				Action = action,
				Timestamp = DateTime.UtcNow
			};

			_context.AuditLogs.Add(auditLog);
			await _context.SaveChangesAsync();

			_logger.LogInformation("Audit log: {Action} for user {UserId}", action, userId);
		}
	}
}
