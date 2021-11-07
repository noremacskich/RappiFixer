using CsvHelper;
using RappiFixer.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace RappiFixer.UseCases
{
    public class LoadInRappiCSVFileUseCase
    {
        internal static List<CSVHeaders> LoadInCSVFile(string fileLocation)
        {

            if (!File.Exists(fileLocation))
            {
                Console.WriteLine("No se puede encontrar el archivo \"Rappi.csv\"");

                throw new FileNotFoundException("unable to load in the rappi file", fileLocation);
            }

            using StreamReader reader = new StreamReader(fileLocation);
            using var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture);

            var records = csvReader.GetRecords<CSVHeaders>();

            return records.ToList();
        }
    }
}
