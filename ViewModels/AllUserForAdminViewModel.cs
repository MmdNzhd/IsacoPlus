using System;
using System.Collections.Generic;
using System.Text;

namespace KaraYadak
{
    public class AllUserForAdminViewModel
    {
        public string Id { get; set; }
        public string Logo { get; set; }
        public string FullName { get; set; }
        public string PhoneNUmber { get; set; }
        public string Email { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string PostalCode { get; set; }
        public string Address { get; set; }
        public string SumOfOrder { get; set; }
        public string CountOfOrder { get; set; }
        public bool IsActive { get; set; }
        
    }
}
