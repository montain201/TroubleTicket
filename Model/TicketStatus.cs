using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Model
{
    public class TicketStatus
    {
        public int TicketStatusId { get; set; }
        public string TicketState { get; set; }
        public string TicketStatusDescription { get; set; }
        public string UserId { get; set; }
        public DateTime CreationDate { get; set; }
        public int TicketId { get; set; }
        public virtual Ticket Ticket { get; set; }
        public ICollection<TicketAttachment> TicketAttachments { get; set; }

    }

    public enum TicketState
    {
        Created,
        UnderCheck,
        Done,
        RoleBack,
        Confirm
    }
}
