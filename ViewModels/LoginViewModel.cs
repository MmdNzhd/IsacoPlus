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
}
