using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

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
                Console.WriteLine(" 1: Lookup Orders");
                Console.WriteLine(" 2: Get Sold Inventory");
                Console.WriteLine(" 3: Exit");

                var menuId = ParseOption();

                if (menuId == -1) continue;

                switch (menuId)
                {
                    case 1: LookupRecords(allRecords); break;
                    case 2: PrintOutInventory(allRecords); break;
                    case 3: inMenu = false; break;
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

            var validIds = new List<long> { 1, 2, 3 };
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

        private void LookupRecords(List<CSVHeaders> allRecords)
        {
            var uniqueRecords = allRecords
                .GroupBy(x => x.order_id)
                .Select(x => new OrderSummary()
                {
                    OrderId = x.First().order_id,
                    UserName = x.First().user,
                    NumberOfProducts = x.Count(),
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
                }

                var lookedupUser = uniqueRecords.FirstOrDefault(x => x.OrderId == orderId);

                if (lookedupUser == null)
                {
                    Console.WriteLine("This order doesn't exist");
                    continue;
                }

                Console.WriteLine($"\r\nThis order was for {lookedupUser.UserName}, and they had {lookedupUser.NumberOfProducts} products : \r\n\r\n{string.Join("\r\n", lookedupUser.Products)}");

            }

        }

        private void PrintOutInventory(List<CSVHeaders> allRecords)
        {
            var products = allRecords
                .Where(x => x.state == "finished")
                .GroupBy(x => x.product)
                .Select(x => new 
                {
                    ProductName = x.First().product,
                    Price = x.Sum(x => x.product_total_price_with_discount),
                    Count = x.Count()                    
                }).ToList();

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("==================================================================================");

            foreach(var product in products)
            {
                Console.WriteLine($"{product.Count} \t {product.Price.ToString("c")} \t {product.ProductName}");
            }


            Console.WriteLine("==================================================================================");

            Console.WriteLine($"\t {products.Sum(x => x.Price).ToString("c")}");

        }
    }
}
