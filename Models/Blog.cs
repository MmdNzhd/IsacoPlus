using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KaraYadak.Models
{
    public class Blog
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ShortDescription { get; set; }
        public string Text { get; set; }
        public string Image { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }

    }
}
