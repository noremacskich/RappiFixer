using RappiFixer.Helpers;
using RappiFixer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RappiFixer.UseCases
{
    public class RepeatCustomersUseCase
    {
        internal static void PrintOutRepeatCustomers(List<RappiDataModel> allRecords, List<ProductCost> productCosts)
        {

            var repeatCustomers = allRecords
                .Where(x => x.OrderState == "finished")
                .GroupBy(x => new { x.UserName })
                .Select(x => new
                {
                    name = x.First().UserName,
                    NumberOfOrders = x.GroupBy(x => x.OrderId).Count(),
                    FirstOrderDate = x.Min(y => y.CreateDate.Date),
                    LastOrderDate = x.Max(y => y.CreateDate.Date)
                });


            foreach(var customer in repeatCustomers.Where(x => x.NumberOfOrders > 1))
            {
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine($"Nombre : {customer.name}");
                Console.WriteLine($"Numero de ordenes : {customer.NumberOfOrders}");
                Console.WriteLine($"Cliente activa entre : {customer.FirstOrderDate.ToLongDateString()} - {customer.LastOrderDate.ToLongDateString()}");
                Console.WriteLine("Preferencias del cliente");


                var records = allRecords.Where(x => x.UserName == customer.name).ToList();
                ProfitHelper.PrintOutProfits(records, productCosts);
            }


        }
    }
}
