using KaraYadak.Models;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace KaraYadak
{
    public class GetAllTicketForCurrectUser
    {
        public int Id { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
        public string Answer { get; set; }
        public TicketPriorityStatus TicketPriorityStatus { get; set; }
        public string CreateDate { get; set; }
        public bool HasAnswer { get; set; }
        public string SenderFullName { get; set; }
        public string ReceiverFullName { get; set; }
        public string SenderId { get; set; }

    }
    public class TicketDetailsForUser: GetAllTicketForAdmin
    {
        /// <summary>
        ///if ticket from admin send user can be answer to this
        /// </summary>
    }
}
