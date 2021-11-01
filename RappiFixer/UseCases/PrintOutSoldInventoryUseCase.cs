﻿using RappiFixer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RappiFixer.UseCases
{
    public static class PrintOutSoldInventoryUseCase
    {
        internal static void PrintOutInventory(List<CSVHeaders> allRecords, List<ProductCost> productCosts)
        {
            var products = allRecords
                .Where(x => x.state == "finished")
                .GroupBy(x => x.product)
                .Select(x => new
                {
                    ProductName = x.First().product,
                    Price = x.Sum(x => x.product_total_price_with_discount),
                    Count = x.Count(),
                    Profit = (productCosts.FirstOrDefault(y => y.PROMOCION.Equals(x.First().product, StringComparison.InvariantCultureIgnoreCase))?.GANACIA ?? 0) * x.Count()
                }).ToList();

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("==================================================================================");

            foreach (var product in products)
            {
                Console.WriteLine($"{product.Count} \t {product.Price:c} \t {product.Profit:c} \t {product.ProductName}");
            }


            Console.WriteLine("==================================================================================");

            Console.WriteLine($"\t {products.Sum(x => x.Price):c} \t {products.Sum(x => x.Profit):c}");

        }
    }
}
