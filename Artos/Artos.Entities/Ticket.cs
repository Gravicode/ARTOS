using System;
using System.Collections.Generic;

namespace Artos.Entities
{
    public class Ticket : AuditAttribute
    {
        public long Id { get; set; }
        public string QRCode { get; set; }
        public string TicketNumber { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public TimeSpan Duration { get; set; }
        public TicketStatus Status { get; set; }
        public string Username { get; set; }
        public long UserId { get; set; }

        public List<TicketHistory> TicketHistories { set; get; }
        public static Ticket GenerateTicket(int hour, int minute, int second) => new Ticket() { Id = 0, TicketNumber = Guid.NewGuid().ToString(), Created = DateTime.Now, Duration = new TimeSpan(hour, minute, second), Status = TicketStatus.Ready };
        
        public bool ActivateTicket(long UserId, string UserName=null)
        {
            this.UserId = UserId;
            this.Username = Username;
            Status = TicketStatus.Active;
            StartDate = DateTime.Now;
            EndDate = StartDate + Duration;
            TicketHistories = new List<TicketHistory>();
            return true;
        }
    }

    public class TicketHistory
    {
        public DateTime ScanDate { get; set; }
        public string QRCode { set; get; }
        public string Remark { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
    }
    public enum TicketStatus { Ready, Active, InActive }
}
