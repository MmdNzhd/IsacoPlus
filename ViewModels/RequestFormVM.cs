using System.ComponentModel.DataAnnotations;
using KaraYadak.Models;

namespace KaraYadak.ViewModels
{
    public class RequestFormVM
    {
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Display(Name = "نام")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Display(Name = " نام خانوادگی" )]
        public string LastName { get; set; }
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [StringLength(maximumLength: 11, MinimumLength = 11)]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string Address { get; set; }
        //[StringLength(maximumLength: 10, MinimumLength = 10,ErrorMessage ="{0} معتبر نیست")]
        //[Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        //[Display(Name = "کد پستی")]
        public string PostalCode { get; set; }
        public string Vahed { get; set; }
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [EmailAddress(ErrorMessage = "{0} معتبر نیست")]
        [Display(Name = "ایمیل")]
        public string Email { get; set; }
        public string Map { get; set; }
        public string Phone { get; set; }
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [Display(Name = "نحوه پرداخت")]
        public PaymentType PaymentType { get; set; }
    }
}
