using RappiFixer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RappiFixer.UseCases
{
    public class LookupOrdersUseCase
    {
        internal static void LookupRecords(List<CSVHeaders> allRecords)
        {
            var uniqueRecords = allRecords
                .GroupBy(x => x.order_id)
                .Select(x => new OrderSummary()
                {
                    OrderId = x.First().order_id,
                    UserName = x.First().user,
                    NumberOfProducts = x.Count(),
                    Status = x.First().state,
                    Products = x.Select(x => x.product).ToList(),
                    Date = DateTime.Parse(x.First().created_at.Substring(0, 20))
                }).ToList();

            var lookingForNumbers = true;

            while (lookingForNumbers)
            {
                Console.WriteLine("\r\nEnter Order Number.  Type \"exit\" to escape.");
                var input = Console.ReadLine();

                input.Trim();

                if (input == "exit")
                {
                    lookingForNumbers = false;
                    continue;
                }

                long orderId;
                if (!long.TryParse(input, out orderId))
                {
                    Console.WriteLine("you did not specify a order id, try again");
                    Console.Beep();
                    Console.Beep();
                    Console.Beep();
                    continue;
                }

                var lookedupUser = uniqueRecords.FirstOrDefault(x => x.OrderId == orderId);

                if (lookedupUser == null)
                {
                    Console.WriteLine("This order doesn't exist");
                    Console.Beep();
                    Console.Beep();
                    continue;
                }

                Console.Beep();

                Console.WriteLine();
                Console.WriteLine($"Nombre : {lookedupUser.UserName}");
                Console.WriteLine($"Order Status : {lookedupUser.Status}");
                Console.WriteLine("=====================================================");
                Console.WriteLine($"{string.Join("\r\n", lookedupUser.Products)}");
                Console.WriteLine("=====================================================");

                Console.WriteLine();
                Console.WriteLine();

            }

        }
    }
}
