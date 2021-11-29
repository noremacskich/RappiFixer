using System;
using System.Collections.Generic;
using System.Text;

namespace RappiFixer.Models
{
    public class RappiDataModel
    {
        public string OrderState { get; set; }
        public long OrderId { get; set; }
        public string UserName { get; set; }
        public DateTime CreateDate { get; set; }
        public string ProductName { get; set; }
        public double Cost { get; set; }
        public int NumberOfUnits { get; set; }
    }
}
