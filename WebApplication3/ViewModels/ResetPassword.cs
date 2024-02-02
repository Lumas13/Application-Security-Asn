using System.ComponentModel.DataAnnotations;

namespace WebApplication3.ViewModels
{
    public class ResetPassword{
        [Required]
        public string UserId { get; set; }

        [Required]
        public string Code { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirmation password is required.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
