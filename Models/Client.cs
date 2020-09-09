using System;
using System.ComponentModel.DataAnnotations;

namespace KaraYadak.Models
{
    public class Client
    {
        public Client()
        {
            CreatedAt = DateTime.Now;
            UpdatedAt = DateTime.Now;
        }
        [Key]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Mobile { get; set; }
        public string Address { get; set; }
        public ClientStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
