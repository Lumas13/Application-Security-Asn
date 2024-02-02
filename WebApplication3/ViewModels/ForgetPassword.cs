using System.ComponentModel.DataAnnotations;

namespace WebApplication3.ViewModels
{
    public class ForgetPassword
    {
        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        public string Email { get; set; }
    }
}
