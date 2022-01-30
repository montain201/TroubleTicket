using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Model
{
    public class TicketAttachment
    {
        public int TicketAttachmentId { get; set; }
        public int TicketStatusId { get; set; }
        public string AttachmentPath { get; set; }
        public TicketStatus TicketStatus { get; set; }
    }
}
