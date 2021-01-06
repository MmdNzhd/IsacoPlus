using System;
using System.Collections.Generic;
using System.Text;

namespace KaraYadak
{
    public class GetAllTicketForAdmin: GetAllTicketForCurrectUser
    {
        public string AnswerDate { get; set; }
        public bool IsSenderSeen { get; set; }
        public string SenderFile { get; set; }
        public string ReceiverFile { get; set; }
        public string SenderId { get; set; }

    }
    public class TicketNotification
    {
        public List<GetAllTicketForAdmin> Tickets { get; set; }
        public int Count { get; set; }
    }
}
