using System.ComponentModel.DataAnnotations;
using DNTPersianUtils.Core;

namespace KaraYadak.ViewModels
{
    public class EditProfileViewModel
    {
        [Required]
        public string FirstName { get; set; }

        public string LastName { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "ایمیل را به درستی وارد کنید")]
        public string Email { get; set; }

        [ValidIranianPhoneNumber(ErrorMessage = "لطفا شماره همراه را به درستی وارد کنید")]
        public string Phone { get; set; }

        public string Username { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "کلمه عبور همخوانی ندارد")]
        public string ConfirmPassword { get; set; }
    }
}
