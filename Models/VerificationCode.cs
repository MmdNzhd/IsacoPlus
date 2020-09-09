using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KaraYadak.Models
{
    public class VerificationCode
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Code { get; set; }
        public DateTime Date { get; set; }
        public VerificationCodeStatus Status { get; set; }
    }
}
