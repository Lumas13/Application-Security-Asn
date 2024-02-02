using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace WebApplication3.ViewModels
{
    public class Register
    {
        [Required(ErrorMessage = "First name is required.")]
        [DataType(DataType.Text)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        [DataType(DataType.Text)]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Credit card is required.")]
        [DataType(DataType.CreditCard)]
        [RegularExpression(@"^\d{16}$", ErrorMessage = "Credit card should contain 16 digits only.")]
        public string CreditCard { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^\d{8}$", ErrorMessage = "Phone number should contain 8 digits only.")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Billing address is required.")]
        [DataType(DataType.Text)]
        public string BillingAddress { get; set; }

        [Required(ErrorMessage = "Shipping address is required.")]
        [DataType(DataType.Text)]
        public string ShippingAddress { get; set; }

        [Required(ErrorMessage = "Email address is required.")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{12,}$", ErrorMessage = "Password should contain upper and lower characters, digits, and special characters.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirmation password is required.")]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public IFormFile PhotoFile { get; set; }
    }
}
