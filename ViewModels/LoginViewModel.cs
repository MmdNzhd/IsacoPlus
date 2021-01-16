using DNTPersianUtils.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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


        [DisplayName("شماره تلفن")]
        [Required(ErrorMessage = "لطفا  {0}  را وارد کنید")]
        //[MaxLength(500, ErrorMessage = "{0} نمیتواند بیشتر از {1} کاراکتر باشد ")]
        //[DataType(DataType/*.*/PhoneNumber)]
        [RegularExpression(@"^(\+98|0)?9\d{9}$",ErrorMessage ="شماره تلفن نامعتبر")]
        public string PhoneNumber { get; set; }

        public bool RememberMe { get; set; }


    }
    public class IdentityRegisterViewModel : IdentityLoginViewModel
    {
        [DisplayName(" نام و نام خانوادگی")]
        [Required(ErrorMessage = "لطفا  {0}  را وارد کنید")]
        public string Nickname { get; set; }
    }

    public class IdentityVerifyViewModel
    {
        public string ReturnUrl { get; set; }
        public string LoginOrRegister { get; set; }
        [Required]
        [RegularExpression(@"^(\+98|0)?9\d{9}$", ErrorMessage = "شماره تلفن نامعتبر")]
        public string PhoneNumber { get; set; }
        [Required]
        [MaxLength(5, ErrorMessage = "کد 5 رقمی نا درست")]
        public string VerificationNumber { get; set; }
    }
}
