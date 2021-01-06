using KaraYadak.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace KaraYadak
{
    public class Ticket
    {
        public int Id { get; set; }

        [DisplayName("موضوع")]
        [Required(ErrorMessage = "لطفا  {0}  را وارد کنید")]
        [MaxLength(200, ErrorMessage = "{0} نمیتواند بیشتر از {1} کاراکتر باشد ")]
        public string Subject { get; set; }

        [DisplayName("متن پیام")]
        [Required(ErrorMessage = "لطفا  {0}  را وارد کنید")]
        [MaxLength(500, ErrorMessage = "{0} نمیتواند بیشتر از {1} کاراکتر باشد ")]
        public string Content { get; set; }
        public string SenderFile { get; set; }
        public string ReceiverFile { get; set; }
        public TicketPriorityStatus TicketPriorityStatus { get; set; }
        public DateTime CreateDate { get; set; }

               public string Answer { get; set; }
        public DateTime? AnswerDate { get; set; }

        public bool IsReciverSeen { get; set; }
        public DateTime? ReceiverSeenDate { get; set; }

        public bool IsSenderSeen { get; set; }
        public DateTime? SenderSeenDate { get; set; }
        public string SenderId { get; set; }
        public ApplicationUser Sender { get; set; }
        public string ReceiverId { get; set; }
        public ApplicationUser Receive { get; set; }


        public TicketStatus? TicketStatus { get; set; }

    }
}
