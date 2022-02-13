using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Model
{
    public class Ticket
    {
        public int TicketId { get; set; }
        public string TicketNo { get; set; }
        public string  TicketType { get; set; }
        public ICollection<TicketStatus> TicketStatuses { get; set; }
    }
    public enum TicketType
    {
        Development,
        Bug
    }
    public class TicketVM
    {
        public int TicketId { get; set; }
        public string TicketType { get; set; }
        public string TicketDescription { get; set; }
        public string Attachment { get; set; }
        public string TicketState { get; set; }
    }
}
