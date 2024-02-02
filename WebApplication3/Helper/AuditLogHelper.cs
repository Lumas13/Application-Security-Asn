using WebApplication3.Model;

namespace WebApplication3.Helper
{
    public class AuditLogHelper
    {
        private readonly ILogger<AuditLogHelper> _logger;
        private readonly AuthDbContext dbContext;

        public AuditLogHelper(
            ILogger<AuditLogHelper> logger, 
            AuthDbContext dbContext)
        {
            _logger = logger;
            this.dbContext = dbContext;
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

            try
            {
                // Add the AuditLog to the database context and save changes
                dbContext.AuditLogs.Add(auditLog);
                await dbContext.SaveChangesAsync();

                _logger.LogInformation("Audit log: {Action} for user {UserId}", action, userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error logging audit information. Action: {Action}, User: {UserId}", action, userId);
            }
        }
    }
}
