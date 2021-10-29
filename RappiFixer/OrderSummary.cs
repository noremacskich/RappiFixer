using System;
using System.Collections.Generic;
using System.Text;

namespace RappiFixer
{
    public class OrderSummary
    {
        public long OrderId { get; set; }
        public string UserName { get; set; }
        public int NumberOfProducts { get; set; }
        public List<string> Products { get; set; }
        public DateTime Date { get; set; }
    }
}
