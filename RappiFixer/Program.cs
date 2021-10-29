using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace RappiFixer
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var allRecords = LoadInCSVFile(args[0]);

            var uniqueRecords = allRecords
                .GroupBy(x => x.order_id)
                .Select(x => new OrderSummary() {
                    OrderId = x.First().order_id,
                    UserName = x.First().user,
                    NumberOfProducts = x.Count(),
                    Products = x.Select(x => x.product).ToList(),
                    Date = DateTime.Parse(x.First().created_at.Substring(0, 20))
                }).ToList();

            Console.WriteLine($"Succesfully read in the CSV file.  You had {uniqueRecords.Count()} orders with a total of {allRecords.Count} products sold for the dates {uniqueRecords.Min(x => x.date).ToLongDateString()} to {uniqueRecords.Max(x => x.Date).ToLongDateString()}");

            LookupRecords(uniqueRecords);

            Console.WriteLine("Teee ammoo mariposa");
            return;
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

        private void LookupRecords(List<OrderSummary> uniqueRecords)
        {
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
    }
}
