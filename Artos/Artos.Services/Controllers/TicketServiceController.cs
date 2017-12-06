using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Artos.Tools;
using Artos.Entities;
using System.Collections.Concurrent;
using Artos.Services.Logics;

namespace Artos.Services.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class TicketServiceController : Controller
    {
        RedisDB db { get; set; }
        static ConcurrentQueue<Ticket> readyTickets;
        static ConcurrentDictionary<long,Ticket> activeTickets;
        public TicketServiceController()
        {
            db = ObjectContainer.Get<RedisDB>();
            if (readyTickets == null)
            {
                readyTickets = new ConcurrentQueue<Ticket>();
                var datas = from c in db.GetAllData<Ticket>()
                            where c.Status == TicketStatus.Ready
                            select c;
                foreach (var item in datas)
                    readyTickets.Enqueue(item);

            }
            if (activeTickets == null)
            {
                activeTickets = new ConcurrentDictionary<long, Ticket>();
                var datas = from c in db.GetAllData<Ticket>()
                            where c.Status == TicketStatus.Active
                            select c;
                foreach (var item in datas)
                    activeTickets.TryAdd(item.UserId, item);
            }
        }

        [HttpGet("[action]")]
        public IActionResult CreateTicket(int count, int hours, int minutes, int seconds)
        {
            var hasil = new OutputData() { IsSucceed = true };
            try
            {
                var listTicket = new List<Ticket>();
                foreach (var i in Enumerable.Range(0, count))
                {
                    var item = Ticket.GenerateTicket(hours, minutes, seconds);
                    item.Id = db.GetSequence<Ticket>();
                    db.InsertData<Ticket>(item);
                    readyTickets.Enqueue(item);
                }
            }
            catch
            {
                hasil.IsSucceed = false;
            }


            return Ok(hasil);
        }
        [HttpGet("[action]")]
        public IActionResult UseTicket(string QRCode,long UserId,double Lng, double Lat, string Remark)
        {
            var hasil = new OutputData() { IsSucceed = true };
            PublicServiceLogic publicctl = new PublicServiceLogic();
            var vehicle = publicctl.GetPublicServiceByQrCode(QRCode);
            
            try
            {
                if (vehicle != null) { 
                bool generateNewTicket = false;
                if (activeTickets.ContainsKey(UserId))
                {
                    //jika masih valid
                    if(DateTime.Now < activeTickets[UserId].EndDate)
                    {
                        activeTickets[UserId].TicketHistories.Add(new TicketHistory() { QRCode = QRCode, ScanDate = DateTime.Now, Latitude = Lat, Longitude = Lng, Remark = Remark });
                        db.InsertData<Ticket>(activeTickets[UserId]);
                        hasil.Data = activeTickets[UserId];
                    }
                    else
                    {
                        //matikan tiket aktif karena sudah lewat end date
                        Ticket OldTicket;
                        activeTickets[UserId].Status = TicketStatus.InActive;
                        db.InsertData<Ticket>(activeTickets[UserId]);
                        bool removed =  activeTickets.TryRemove(UserId,out OldTicket);
                        if(removed) generateNewTicket = true;
                    }

                    }
                    else
                    {
                        generateNewTicket = true;

                    }

                    if (generateNewTicket)
                    {
                        //check saldo user

                        EMoneyLogic ctl = new EMoneyLogic();
                        var saldo = ctl.CheckBalance(UserId);
                        if (saldo!=null)
                        {
                            
                            if (saldo.Balance >= vehicle.Price)
                            {
                                Ticket TicketOut;
                                if (readyTickets.TryDequeue(out TicketOut))
                                {
                                    TicketOut.ActivateTicket(UserId);
                                    TicketOut.TicketHistories.Add(new TicketHistory() { QRCode = QRCode, ScanDate = DateTime.Now, Latitude = Lat, Longitude = Lng, Remark = Remark });
                                    activeTickets.TryAdd(UserId, TicketOut);
                                    db.InsertData<Ticket>(activeTickets[UserId]);
                                    hasil.Data = activeTickets[UserId];
                                }
                                else
                                {
                                    hasil.IsSucceed = false;
                                    hasil.ErrorMessage = "There is no ticket available...";
                                }
                                ctl.CutBalance(UserId, vehicle.Price);
                            }
                        }

                       
                    }
                }

              
            }
            catch
            {
                hasil.IsSucceed = false;
            }


            return Ok(hasil);
        }
        [HttpGet("[action]")]
        public IActionResult GetAvailableTicketCount()
        {
            var hasil = new OutputData() { IsSucceed = true, Data = 0 };
            try
            {
                hasil.Data =  readyTickets.Count;
            }
            catch
            {
                hasil.IsSucceed = false;
            }


            return Ok(hasil);
        }

        [HttpGet("[action]")]
        public IActionResult GetActiveTicketCount()
        {
            var hasil = new OutputData() { IsSucceed = true, Data = 0 };
            try
            {
                hasil.Data = activeTickets.Count;
            }
            catch
            {
                hasil.IsSucceed = false;
            }


            return Ok(hasil);
        }
    }
}
