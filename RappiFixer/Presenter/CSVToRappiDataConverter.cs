using System;
using System.Collections.Generic;
using RappiFixer.Models;
using System.Linq;
using System.Globalization;

namespace RappiFixer.Presenter
{
    public class CSVToRappiDataConverter
    {
        public List<RappiDataModel> ConvertToRappiDataModel(List<CSVHeaders> csvHeaders)
        {
            return csvHeaders.Select(x => new RappiDataModel(){
                Cost = x.product_total_price_with_discount,
                CreateDate = ConvertToLocalDateTime(x.created_at.Substring(0, 19)).Date,
                NumberOfUnits = x.product_units,
                OrderId = x.order_id,
                Product = x.product,
                UserName = x.user,
                OrderState = x.state
            }).ToList();
        }

        private static DateTime ConvertToLocalDateTime(string date){
            var parsedDate = DateTime.ParseExact(date.Substring(0, 19), "yyyy-MM-dd HH:mm:ss", DateTimeFormatInfo.InvariantInfo);
            return DateTime.SpecifyKind(parsedDate, DateTimeKind.Utc).ToLocalTime();
        }
    }
}