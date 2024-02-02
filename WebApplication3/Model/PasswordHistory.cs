using System.ComponentModel.DataAnnotations.Schema;
using WebApplication3.Model;

public class PasswordHistory
{
    public int Id { get; set; }

    public string UserId { get; set; }

    [ForeignKey("UserId")]
    public ApplicationUser User { get; set; }

    public string PasswordHash { get; set; }

    public DateTime DateChanged { get; set; }
}
