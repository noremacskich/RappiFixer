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

            var rappi = new RappiOrderApp();

            rappi.StartApp(args);

            return 1;
        }

       
    }
}
