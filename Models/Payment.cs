using KaraYadak.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KaraYadak
{
    public class Payment
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public decimal Discount { get; set; }
        public decimal FinallyAmountWithTax { get; set; }

        public bool IsSucceed { get; set; }
        public string InvoiceKey { get; set; }
        public string TransactionCode { get; set; }
        public DateTime Date { get; set; }
        public string TrackingNumber { get; set; }
        public string ErrorDescription { get; set; }
        public string ErrorCode { get; set; }
        public bool IsBackMOney { get; set; }
        public Transaction Transaction { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public int? ShoppingCartId { get; set; }
        public ShoppingCart ShoppingCart { get; set; }
    }
}
