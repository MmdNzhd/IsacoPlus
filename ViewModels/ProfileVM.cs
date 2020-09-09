using DNTPersianUtils.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace KaraYadak.ViewModels
{
    public class ProfileVM
    {
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Display(Name = "نام")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Display(Name = " نام خانوادگی")]
        public string LastName { get; set; }
        [Display(Name = "آدرس")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string Address { get; set; }
        [ValidIranianMobileNumber(ErrorMessage = "{0} معتبر نیست")]
        //[StringLength(maximumLength: 11, MinimumLength = 11, ErrorMessage = "{0} معتبر نیست")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Display(Name = " تلفن همراه")]
        public string PhoneNumber { get; set; }
        [ValidIranianNationalCode(ErrorMessage = "{0} معتبر نیست")]
        //[StringLength(maximumLength: 10, MinimumLength = 10, ErrorMessage = "{0} معتبر نیست")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Display(Name = "کد ملی")]
        public string NationalCode { get; set; }
        [StringLength(maximumLength: 11, MinimumLength = 8, ErrorMessage = "{0} معتبر نیست")]
        //[Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Display(Name = "تلفن ثابت")]
        public string Phone { get; set; }
        public string CallbackUrl { get; set; }
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [EmailAddress(ErrorMessage = "{0} معتبر نیست")]
        [Display(Name = "ایمیل")]
        public string Email { get; set; }
        [StringLength(maximumLength: 10, MinimumLength = 10)]
        public string PostalCode { get; set; }
        [StringLength(maximumLength: 10)]
        [Display(Name = "جنسیت")]
        public string Gender { get; set; }
        [Display(Name = "تصویر پروفایل")]
        public string ImageProfile { get; set; }
        [ValidIranShetabNumber(ErrorMessage = "{0} معتبر نیست")]
        //[StringLength(maximumLength: 16, MinimumLength = 16, ErrorMessage = "{0} معتبر نیست")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Display(Name = " کارت بانکی")]
        public string  CartNumber { get; set; }
    }

}
