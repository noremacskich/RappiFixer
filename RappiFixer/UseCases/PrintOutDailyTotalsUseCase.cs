using RappiFixer.Helpers;
using RappiFixer.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RappiFixer.UseCases
{
    public class PrintOutDailyTotalsUseCase
    {

        internal static void PrintOutDailyTotals(List<RappiDataModel> allRecords, List<ProductCost> productCosts)
        {

            var repeatCustomers = allRecords
                .Where(x => x.OrderState == "finished")
                .GroupBy(x => x.CreateDate.Date)
                .Select(x => new
                {
                    OrderDate = x.Key,
                    Count = x.Sum(x => x.NumberOfUnits),
                    Profit = (productCosts.FirstOrDefault(y => y.PROMOCION.Trim().Equals(x.First().ProductName, StringComparison.InvariantCultureIgnoreCase))?.GANACIA ?? 0) * x.Sum(x => x.NumberOfUnits),
                    Cost = x.Sum(x => x.Cost),
                    AllOrderItems = x.ToList(),
                    NumberOfOrders = x.GroupBy(x => x.OrderId).Count()
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
