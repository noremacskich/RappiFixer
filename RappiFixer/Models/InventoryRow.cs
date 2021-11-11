using System;
using System.Collections.Generic;
using System.Text;

namespace RappiFixer.Models
{
    public class InventoryRow
    {
        public string ProductName { get; set; }
        public string Tipo { get; set; }
        public double Profit { get; set; }
        public double Cost { get; set; }
        public int Count { get; set; }
    }
}
