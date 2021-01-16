using KaraYadak.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace KaraYadak
{
    public class ChangeOrderTypeByWarehousingAdminViewModel
    {
        public OrderLevel OrderLevel { get; set; }

        [DisplayName("کد رهگیری پستی")]
        [MaxLength(50, ErrorMessage = "{0} نمیتواند بیشتر از {1} کاراکتر باشد ")]
        public string PostTrackingNumber { get; set; }
        public int ShopingCartId { get; set; }
        public int PaymentId { get; set; }
    }
}
