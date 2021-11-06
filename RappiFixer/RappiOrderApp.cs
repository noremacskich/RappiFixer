using CsvHelper;
using RappiFixer.Models;
using RappiFixer.UseCases;
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
            var productCosts = LoadInProductCostsCSV(args[1]);

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
                    case 1: LookupOrdersUseCase.LookupRecords(allRecords); break;
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


        private List<ProductCost> LoadInProductCostsCSV(string fileLocation)
        {

            if (!File.Exists(fileLocation))
            {
                Console.WriteLine("Unable to find the csv file");
                throw new FileNotFoundException("unable to find the csv file", fileLocation);
            }

            var utf8NoBom = new UTF8Encoding(false);
            using (var reader = new StreamReader(fileLocation, utf8NoBom))
            {
                reader.Read();
                if (Equals(reader.CurrentEncoding, utf8NoBom))
                {
                    //Console.WriteLine("No BOM");
                    using var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture);

                    var records = csvReader.GetRecords<ProductCost>();
                    return records.ToList();
                }
                else
                {
                    //Console.WriteLine("BOM detected");

                    using var csvReader = new CsvReader(reader, new CsvHelper.Configuration.CsvConfiguration(CultureInfo.CurrentCulture) { 
                        Delimiter = ";",
                        Encoding = GetEncoding(fileLocation)
                    });
                    var records = csvReader.GetRecords<ProductCost>();
                    return records.ToList();
                }
            }
            

        }

        /// <summary>
        /// Determines a text file's encoding by analyzing its byte order mark (BOM).
        /// Defaults to ASCII when detection of the text file's endian-ness fails.
        /// </summary>
        /// <param name="filename">The text file to analyze.</param>
        /// <returns>The detected encoding.</returns>
        private static Encoding GetEncoding(string filename)
        {
            // Read the BOM
            var bom = new byte[4];
            using (var file = new FileStream(filename, FileMode.Open, FileAccess.Read))
            {
                file.Read(bom, 0, 4);
            }

            // Analyze the BOM
            if (bom[0] == 0x2b && bom[1] == 0x2f && bom[2] == 0x76) return Encoding.UTF7;
            if (bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf) return Encoding.UTF8;
            if (bom[0] == 0xff && bom[1] == 0xfe) return Encoding.Unicode; // UTF-16LE
            if (bom[0] == 0xfe && bom[1] == 0xff) return Encoding.BigEndianUnicode; // UTF-16BE
            if (bom[0] == 0 && bom[1] == 0 && bom[2] == 0xfe && bom[3] == 0xff) return Encoding.UTF32;
            return Encoding.Default; // **Changed this line**
        }

    }
}
