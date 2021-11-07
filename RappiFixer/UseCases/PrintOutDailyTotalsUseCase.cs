using RappiFixer.Helpers;
using RappiFixer.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RappiFixer.UseCases
{
    public class PrintOutDailyTotalsUseCase
    {

        internal static void PrintOutDailyTotals(List<CSVHeaders> allRecords, List<ProductCost> productCosts)
        {

            var repeatCustomers = allRecords
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


            foreach (var customer in repeatCustomers.OrderBy(x => x.OrderDate))
            {
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine($"Date : {customer.OrderDate.ToLongDateString()}");
                Console.WriteLine($"Number Of Orders : {customer.NumberOfOrders}");
                ProfitHelper.PrintOutProfits(customer.AllOrderItems, productCosts);
            }


        }

    }
}
