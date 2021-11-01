using CsvHelper;
using RappiFixer.Models;
using RappiFixer.UseCases;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace RappiFixer
{
    public class RappiOrderApp
    {
        public void StartApp(string[] args)
        {
            var allRecords = LoadInCSVFile(args[0]);

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
                Console.WriteLine(" 3: Checkout Item");


                var menuId = ParseOption();

                if (menuId == -1) continue;

                switch (menuId)
                {
                    case 0: inMenu = false; break;
                    case 1: LookupOrdersUseCase.LookupRecords(allRecords); break;
                    case 2: PrintOutSoldInventoryUseCase.PrintOutInventory(allRecords); break;
                    case 3: CheckoutItem(); break;
                }

                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine();
            }

            Console.WriteLine("Teee ammoo mariposa");
        }

        private void CheckoutItem()
        {
            Console.WriteLine("\r\nScan barcode to add item to sold list.  Type \"exit\" to escape.");
            var input = Console.ReadLine();
            input.Trim();
            
            // Lookup sku number to see if it already exists

                // If so, display it

            // If not, ask to put it back into the system

            // Ask for quantity sold

            // Update csv

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

        private List<CSVHeaders> LoadInCSVFile(string fileLocation)
        {

            if (!File.Exists(fileLocation))
            {
                Console.WriteLine("Unable to find the csv file");
                throw new FileNotFoundException("unable to find the csv file", fileLocation);
            }

            using StreamReader reader = new StreamReader(fileLocation);
            using var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture);

            var records = csvReader.GetRecords<CSVHeaders>();

            return records.ToList();
        }

        


    }
}
