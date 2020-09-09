using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace KaraYadak.Models
{
    public class Comment
    {

        public int Id { get; set; }
        public Product Product { get; set; }
        public string ProductCode { get; set; }
        public DateTime Date { get; set; }
        public string Title { get; set; }
        public string Username { get; set; }
        [Required]
        public string Text { get; set; }
        public CommentStatus Status { get; set; }
        public int Rate { get; set; }
    }
}
