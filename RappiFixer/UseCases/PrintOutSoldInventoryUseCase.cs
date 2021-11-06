using RappiFixer.Helpers;
using RappiFixer.Models;
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
            var records = allRecords.Where(x => x.state == "finished").ToList();
            ProfitHelper.PrintOutProfits(records, productCosts);

        }
    }
}
