using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace KaraYadak.Models
{
    public class ContactUsMessage
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Text { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime CreateAt { get; set; }
    }
}
