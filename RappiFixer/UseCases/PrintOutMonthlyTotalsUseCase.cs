using RappiFixer.Helpers;
using RappiFixer.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RappiFixer.UseCases
{
    public class PrintOutMonthlyTotalsUseCase
    {

        internal static void PrintOutMonthlyTotals(List<CSVHeaders> allRecords, List<ProductCost> productCosts)
        {

            var calendarDays = allRecords
                .Where(x => x.state == "finished")
                .GroupBy(x => DateTime.Parse(x.created_at.Substring(0, 10)).Date)
                .Select(x => new
                {
                    OrderDate = x.Key,
                    Count = x.Sum(x => x.product_units),
                    Profit = (productCosts.FirstOrDefault(y => y.PROMOCION.Trim().Equals(x.First().product, StringComparison.InvariantCultureIgnoreCase))?.GANACIA ?? 0) * x.Sum(x => x.product_units),
                    Cost = x.Sum(x => x.product_total_price_with_discount),
                    AllOrderItems = x.ToList(),
                    NumberOfOrders = x.GroupBy(x => x.order_id).Count()
                });


            var startDate = calendarDays.Min(x => x.OrderDate);
            var endDate = calendarDays.Max(x => x.OrderDate);

            while(startDate <= endDate)
            {
                var date = startDate.ToLongDateString();
                var numberOfOrders = 0;
                double profit = 0;
                double cost = 0;

                var lookedUpDate = calendarDays.FirstOrDefault(x => x.OrderDate.Date == startDate.Date);

                if(lookedUpDate != null)
                {
                    numberOfOrders = lookedUpDate.NumberOfOrders;
                    profit = lookedUpDate.Profit;
                    cost = lookedUpDate.Cost;
                }

                Console.WriteLine($"Date : {date}");
                Console.WriteLine($"Number Of Orders : {numberOfOrders}");
                Console.WriteLine($"Cost : {cost:c}");
                Console.WriteLine($"Profit : {profit:c}");
                Console.WriteLine();
                Console.WriteLine();

                startDate = startDate.AddDays(1);
            }



        }

    }
}
