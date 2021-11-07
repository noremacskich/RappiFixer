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
            try
            {
                var rappi = new RappiOrderApp();

                rappi.StartApp(args);


            }catch(Exception ex)
            {
                Console.WriteLine(ex.Message);

                Console.WriteLine("Pulse cualquier tecla para continuar");
                Console.ReadKey();
            }

            return;

        }


    }
}
