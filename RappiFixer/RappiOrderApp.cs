using RappiFixer.UseCases;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RappiFixer
{
    public class RappiOrderApp
    {
        public void StartApp(string[] args)
        {
            var allRecords = LoadInRappiCSVFileUseCase.LoadInCSVFile(args[0]);
            var productCosts = LoadInProductCostsUseCase.LoadInProductCostsCSV(args[1]);

            var uniqueRecords = allRecords
                .GroupBy(x => x.order_id)
                .Select(x => new
                {
                    Date = DateTime.Parse(x.First().created_at.Substring(0, 20))
                })
                .ToList();

            Console.WriteLine($"Succesfully read in the CSV file.  You had {uniqueRecords.Count()} orders with a total of {allRecords.Count} products sold for the dates {uniqueRecords.Min(x => x.Date).ToLongDateString()} to {uniqueRecords.Max(x => x.Date).ToLongDateString()}");

            var inMenu = true;
            while (inMenu)
            {
                Console.WriteLine("====================================");
                Console.WriteLine("Please choose an option:");
                Console.WriteLine(" 0: Exit");
                Console.WriteLine(" 1: Lookup Orders");
                Console.WriteLine(" 2: Get Sold Inventory");


                var menuId = ParseOption();

                if (menuId == -1) continue;

                switch (menuId)
                {
                    case 0: inMenu = false; break;
                    case 1: LookupOrdersUseCase.LookupRecords(allRecords, productCosts); break;
                    case 2: PrintOutSoldInventoryUseCase.PrintOutInventory(allRecords, productCosts); break;
                }

                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
            }

            Console.WriteLine("Teee ammoo mariposa");
        }

        private long ParseOption()
        {
            var input = Console.ReadLine();

            input.Trim();

            long menuId;
            if (!long.TryParse(input, out menuId))
            {
                Console.WriteLine("Not a valid menu option, please try again.");
                return -1;
            }

            var validIds = new List<long> { 0, 1, 2, 3 };
            if (!validIds.Contains(menuId))
            {
                Console.WriteLine("You did not enter a valid menu number");
                return -1;
            }

            return menuId;
        }
    }
}
