using RappiFixer.Helpers;
using RappiFixer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RappiFixer.UseCases
{
    public class InventoryTypeUseCase
    {
        internal static void PrintOutInventoryTypes(List<CSVHeaders> allRecords, List<ProductCost> productCosts)
        {

            var combinationRecords = allRecords.Where(x => x.state == "finished").Select(x => new
            {
                rappiRecord = x,
                Tipo = productCosts.FirstOrDefault(y => y.PROMOCION.Trim().Equals(x.product, StringComparison.InvariantCultureIgnoreCase))?.TIPO ?? "No Tippo"
            });


            var productCategories = combinationRecords
                .GroupBy(x => new { x.Tipo })
                .Select(x => new
                {
                    records = x.Select(x => x.rappiRecord),
                    NumberOfOrders = x.Select(x => x.rappiRecord).Sum(x => x.product_units),
                    Tipo = x.Key.Tipo,
                });


            foreach(var category in productCategories.OrderByDescending(x => x.NumberOfOrders))
            {
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine($"Tipo : {category.Tipo}");
                ProfitHelper.PrintOutProfits(category.records.ToList(), productCosts);
            }


        }
    }
}
