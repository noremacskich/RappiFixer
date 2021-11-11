using System;
using System.Collections.Generic;
using System.Text;

namespace RappiFixer.Models
{
    public class CSVHeaders
    {
        public string state { get; set; }
        public long order_id { get; set; }
        public string user { get; set; }
        public string created_at { get; set; }
        public string product { get; set; }
        public double product_total_price_with_discount { get; set; }
        public int product_units { get; set; }
    }
}
