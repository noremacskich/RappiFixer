using CsvHelper;
using RappiFixer.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace RappiFixer.UseCases
{
    public class LoadInProductCostsUseCase
    {
        internal static List<ProductCost> LoadInProductCostsCSV(string fileLocation)
        {
            if (!File.Exists(fileLocation))
            {
                throw new FileNotFoundException("No se puede encontrar el archivo \"Costo.csv\"", fileLocation);
            }

            var utf8NoBom = new UTF8Encoding(false);
            using (var reader = new StreamReader(fileLocation, utf8NoBom))
            {
                reader.Read();

                using var csvReader = new CsvReader(reader, new CsvHelper.Configuration.CsvConfiguration(CultureInfo.CurrentCulture)
                {
                    Delimiter = ";",
                    Encoding = GetEncoding(fileLocation)
                });
                var records = csvReader.GetRecords<ProductCost>();
                return records.ToList();
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
