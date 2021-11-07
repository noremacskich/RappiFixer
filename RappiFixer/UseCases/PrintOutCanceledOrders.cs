using RappiFixer.Helpers;
using RappiFixer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RappiFixer.UseCases
{
    public static class PrintOutCanceledInventoryUseCase
    {
        internal static void PrintOutInventory(List<CSVHeaders> allRecords, List<ProductCost> productCosts)
        {
            var records = allRecords.Where(x => x.state == "canceled").ToList();
            ProfitHelper.PrintOutProfits(records, productCosts);

        }
    }
}
