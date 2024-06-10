using System.ComponentModel.DataAnnotations;

namespace AppAPI.Models
{
    public class Accounts
    {
        [Required]
        [StringLength(256, MinimumLength = 6, ErrorMessage = "Độ dài tối thiểu là 6 kí tự và tối đa là 256 kí tự")]
        public string Username { get; set; }

        [Required]
        [StringLength(256, MinimumLength = 6, ErrorMessage = "Độ dài tối thiểu là 6 kí tự và tối đa là 256 kí tự")]
        public string Password { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        public string Email { get; set; }

        [Required]
        [RegularExpression("^(\\+\\d{1,2}\\s)?\\(?\\d{3}\\)?[\\s.-]\\d{3}[\\s.-]\\d{4}$",
            ErrorMessage = "Số điện thoại phải đúng định dạng xxx-xxx-xxxx")]
        public string Phone { get; set; }
        public string Address { get; set; }
    }
}
