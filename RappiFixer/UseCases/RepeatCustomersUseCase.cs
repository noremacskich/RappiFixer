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
        internal static void PrintOutRepeatCustomers(List<CSVHeaders> allRecords, List<ProductCost> productCosts)
        {

            var repeatCustomers = allRecords.GroupBy(x => new { x.user })
                .Select(x => new
                {
                    name = x.First().user,
                    NumberOfOrders = x.GroupBy(x => x.order_id).Count(),
                    FirstOrderDate = x.Min(y => DateTime.Parse(y.created_at.Substring(0, 20))),
                    LastOrderDate = x.Max(y => DateTime.Parse(y.created_at.Substring(0, 20)))
                });


            foreach(var customer in repeatCustomers.Where(x => x.NumberOfOrders > 1))
            {
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine($"Nombre : {customer.name}");
                Console.WriteLine($"Numero de ordenes : {customer.NumberOfOrders}");
                Console.WriteLine($"Cliente activa entre : {customer.FirstOrderDate.ToLongDateString()} - {customer.LastOrderDate.ToLongDateString()}");
                Console.WriteLine("Preferencias del cliente");


                var records = allRecords.Where(x => x.user == customer.name).ToList();
                ProfitHelper.PrintOutProfits(records, productCosts);
            }


        }
    }
}
