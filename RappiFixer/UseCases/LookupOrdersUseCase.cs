﻿using RappiFixer.Helpers;
using RappiFixer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RappiFixer.UseCases
{
    public class LookupOrdersUseCase
    {
        internal static void LookupRecords(List<CSVHeaders> allRecords, List<ProductCost> productCosts)
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
                Console.WriteLine("\r\nIngrese el número de pedido. Escriba \"salir\" para escapar.");
                var input = Console.ReadLine();

                input.Trim();

                if (input == "salir")
                {
                    lookingForNumbers = false;
                    continue;
                }

                long orderId;
                if (!long.TryParse(input, out orderId))
                {
                    Console.WriteLine("No especificó un ID de pedido, inténtelo de nuevo");
                    Console.Beep();
                    Console.Beep();
                    Console.Beep();
                    continue;
                }

                var lookedupUser = uniqueRecords.FirstOrDefault(x => x.OrderId == orderId);

                if (lookedupUser == null)
                {
                    Console.WriteLine("Esta orden no existe");
                    Console.Beep();
                    Console.Beep();
                    continue;
                }

                Console.Beep();

                Console.WriteLine();
                Console.WriteLine($"Nombre             : {lookedupUser.UserName}");
                Console.WriteLine($"Estado de la orden : {lookedupUser.Status}");

                var userProducts = allRecords.Where(x => x.order_id == orderId).ToList();
                ProfitHelper.PrintOutProfits(userProducts, productCosts);


            }

        }
    }
}
