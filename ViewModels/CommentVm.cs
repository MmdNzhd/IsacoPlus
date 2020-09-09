using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KaraYadak.ViewModels
{
    public class CommentVm
    {
        public int Id { get; set; }
        public string code { get; set; }
        public string UserFullName { get; set; }
        public string Text { get; set; }
        public int Rate { get; set; }

    }
}
