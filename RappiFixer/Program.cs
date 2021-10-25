using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace RappiFixer
{
    class Program
    {
        static void Main(string[] args)
        {

            if (!File.Exists(args[0]))
            {
                Console.WriteLine("Unable to find the csv file");
                return;
            }

            using StreamReader reader = new StreamReader(args[0]);
            using var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture);

            var records = csvReader.GetRecords<CSVHeaders>();

            var allRecords = records.ToList();

            var uniqueRecords = allRecords
                .GroupBy(x => x.order_id)
                .Select(x => new {
                    orderId = x.First().order_id,
                    userName = x.First().user,
                    numberOfProducts = x.Count(),
                    products = x.Select(x => x.product).ToList(),
                    date = DateTime.Parse(x.First().created_at.Substring(0, 20))

                }).ToList();


            var lookingForNumbers = true;

            Console.WriteLine($"Succesfully read in the CSV file.  You had {uniqueRecords.Count()} orders with a total of {allRecords.Count} products sold for the dates {uniqueRecords.Min(x => x.date).ToLongDateString()} to {uniqueRecords.Max(x => x.date).ToLongDateString()}");
            while (lookingForNumbers)
            {
                Console.WriteLine("\r\nEnter Order Number.  Type \"exit\" to escape.");
                var input = Console.ReadLine();

                input.Trim();

                if(input == "exit")
                {
                    lookingForNumbers = false;
                    continue;
                }

                long orderId;
                if (!long.TryParse(input, out orderId))
                {
                    Console.WriteLine("you did not specify a order id, try again");
                }

                var lookedupUser = uniqueRecords.FirstOrDefault(x => x.orderId == orderId);

                if(lookedupUser == null)
                {
                    Console.WriteLine("This order doesn't exist");
                    continue;
                }

                Console.WriteLine($"\r\nThis order was for {lookedupUser.userName}, and they had {lookedupUser.numberOfProducts} products : \r\n\r\n{string.Join("\r\n", lookedupUser.products)}");

            }

            Console.WriteLine("Teee ammoo mariposa");
            return;
        }
    }
}
