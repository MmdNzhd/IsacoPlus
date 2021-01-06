using KaraYadak.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace KaraYadak
{
    public class CreateTiket
    {
        [DisplayName("موضوع")]
        [Required(ErrorMessage = "لطفا  {0}  را وارد کنید")]
        [MaxLength(100, ErrorMessage = "{0} نمیتواند بیشتر از {1} کاراکتر باشد ")]
        public string Subject { get; set; }

        [DisplayName("متن پیام")]
        [Required(ErrorMessage = "لطفا  {0}  را وارد کنید")]
        [MaxLength(500, ErrorMessage = "{0} نمیتواند بیشتر از {1} کاراکتر باشد ")]
        public string Content { get; set; }
        public TicketPriorityStatus TicketPriorityStatus { get; set; }
        public IFormFile File { get; set; }
       
        public List<string> UserId { get; set; }
      

    }
    public class TicketAdminAnswer
    {
        public int Id { get; set; }
        [DisplayName("متن پاسخ")]
        [Required(ErrorMessage = "لطفا  {0}  را وارد کنید")]
        [MaxLength(500, ErrorMessage = "{0} نمیتواند بیشتر از {1} کاراکتر باشد ")]

        public string Answer { get; set; }
        public IFormFile AnswerFile{ get; set; }
    }
}
