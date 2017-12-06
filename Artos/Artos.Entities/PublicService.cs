using System;
using System.Collections.Generic;
using System.Text;

namespace Artos.Entities
{
    public class PublicService
    {
        public long Id { get; set; }
        public string VehicleNumber { get; set; }
        public ModaTypes TipeModa { get; set; }
        public string Rute { get; set; }
        public string QRCode { get; set; }
        public StatusModa Status { get; set; }
        public Int64 Price { set; get; }
        public void GenerateQRCode()
        {
            if (string.IsNullOrEmpty(QRCode))
            {
                QRCode = $"{(int)TipeModa}-{VehicleNumber}-{Rute}";
            }
        }
    }
    public enum StatusModa
    {
        Active, InActive, Maintenance
    }
    public enum ModaTypes
    {
        TransJakarta, Angkot, BusKota, MRT, LRT, Sepeda, OjekOnline
    }
}
