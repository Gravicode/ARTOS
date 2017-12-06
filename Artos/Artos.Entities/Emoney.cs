using System;
using System.Collections.Generic;
using System.Text;

namespace Artos.Entities
{
    public class EMoney
    {
        public long Id { get; set; }
        public string Number { get; set; }
        public string Penerbit { get; set; }
        public string Tipe { get; set; }
        public long OwnerId { get; set; }
        public Int64 Balance { get; set; }
        public List<ScanTransaction> TransactionList { get; set; }
        public bool IsActive { get; set; }   
        public bool IsDefault { get; set; }
    }

    public class ScanTransaction
    {
        public DateTime ScanDate { get; set; }
        public int Amount { get; set; }
        public string Remark { get; set; }
    }
}
