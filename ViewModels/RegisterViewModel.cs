using System.ComponentModel.DataAnnotations;

namespace SDMNG.ViewModels
{
    public class RegisterViewModel
    {

        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress]
        public string Email { get; set; }
        [Required(ErrorMessage = "Address is required.")]
        public string Address { get; set; }
        [Required(ErrorMessage = "ZipCode is required.")]
        public string ZipCode { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm Password is required.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}
