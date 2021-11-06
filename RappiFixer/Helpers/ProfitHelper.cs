using RappiFixer.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RappiFixer.Helpers
{
    public class ProfitHelper
    {
        public static void PrintOutProfits(List<CSVHeaders> allRecords, List<ProductCost> productCosts)
        {
            var products = allRecords
                .GroupBy(x => x.product)
                .Select(x => new InventoryRow()
                {
                    ProductName = x.First().product,
                    Cost = x.Sum(x => x.product_total_price_with_discount),
                    Count = x.Sum(x => x.product_units),
                    Profit = (productCosts.FirstOrDefault(y => y.PROMOCION.Trim().Equals(x.First().product, StringComparison.InvariantCultureIgnoreCase))?.GANACIA ?? 0) * x.Sum(x => x.product_units)
                }).ToList();


            const int costSpacing = 12;
            const int countSpacing = 9;

            Console.WriteLine("==================================================================================");
            Console.WriteLine($"{"Cantidad",countSpacing} {"Costo",costSpacing} {"Lucro",costSpacing} \t Nombre");
            Console.WriteLine("==================================================================================");

            foreach (var product in products.OrderByDescending(x => x.Count).ThenBy(x => x.ProductName))
            {
                Console.WriteLine($"{product.Count,countSpacing} {product.Cost,costSpacing:C} {product.Profit,costSpacing:C} \t {product.ProductName}");
            }


            Console.WriteLine("==================================================================================");

            Console.WriteLine($"{products.Sum(x => x.Count),countSpacing} {products.Sum(x => x.Cost),costSpacing:c} {products.Sum(x => x.Profit),costSpacing:c}");
        }
    }
}
