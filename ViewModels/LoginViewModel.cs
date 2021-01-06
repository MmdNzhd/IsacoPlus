using DNTPersianUtils.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace KaraYadak.ViewModels
{
    public class LoginViewModel
    {
        public string ReturnUrl { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }

        //[Required]
        [RegularExpression("([1-9][0-9]*)")]
        public string Captcha { get; set; }
    }


    public class IdentityLoginViewModel
    {
        public string ReturnUrl { get; set; }
        public string LoginOrRegister { get; set; }


        [Required]
        //[DataType(DataType/*.*/PhoneNumber)]
        [ValidIranianPhoneNumber(ErrorMessage = "شماره تماس نامعتبر")]
        public string PhoneNumber { get; set; }

        public bool RememberMe { get; set; }


    }
    public class IdentityRegisterViewModel : IdentityLoginViewModel
    {
        [Required]
        public string Nickname { get; set; }
    }

    public class IdentityVerifyViewModel 
    {
        public string ReturnUrl { get; set; }
        public string LoginOrRegister { get; set; }
        [Required]
        [ValidIranianPhoneNumber(ErrorMessage = "شماره تماس نامعتبر")]
        public string PhoneNumber { get; set; }
        [Required]
        [MaxLength(4,ErrorMessage ="کد 4 رقمی نا درست")]
        public string VerificationNumber { get; set; }
    }
}
