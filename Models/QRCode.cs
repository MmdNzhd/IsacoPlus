using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace KaraYadak.Models
{
    public class QRCode
    {
        public int Id { get; set; }
        public string QR { get; set; }
        public string Code { get; set; }
        public DateTime CreateAt { get; set; }
        public QrUse Use { get; set; }
    }
}
