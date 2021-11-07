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
            if(args == null || args.Length == 0)
            {
                Console.WriteLine("De forma predeterminada, buscará el archivo que descargó de Rappi como \"Rappi.csv\".");
                Console.WriteLine("De forma predeterminada, buscará el archivo con todos los precios como \"Costo.csv\".");
            }

            string rappiFileLocation = Environment.CurrentDirectory + "\\Rappi.csv";
            string productCostsFileLocation = Environment.CurrentDirectory + "\\Costo.csv";
            if (args != null && args.Length >= 1)
            {
                rappiFileLocation = args[0];
            }

            if (args != null && args.Length == 2)
            {
                productCostsFileLocation = args[1];
            }

            var allRecords = LoadInRappiCSVFileUseCase.LoadInCSVFile(rappiFileLocation);
            var productCosts = LoadInProductCostsUseCase.LoadInProductCostsCSV(productCostsFileLocation);

            var uniqueRecords = allRecords
                .GroupBy(x => x.order_id)
                .Select(x => new
                {
                    Date = DateTime.Parse(x.First().created_at.Substring(0, 20))
                })
                .ToList();

            Console.WriteLine($"Tuvo {uniqueRecords.Count()} pedidos con un total de {allRecords.Count} productos vendidos para las fechas del {uniqueRecords.Min(x => x.Date).ToLongDateString()} al {uniqueRecords.Max(x => x.Date).ToLongDateString()}");

            var inMenu = true;
            while (inMenu)
            {
                Console.WriteLine("====================================");
                Console.WriteLine("Por favor, elija una opción:");
                Console.WriteLine(" 0: Salida");
                Console.WriteLine(" 1: Órdenes de búsqueda");
                Console.WriteLine(" 2: Obtener inventario vendido");
                Console.WriteLine(" 3: Obtener inventario cancelado");
                Console.WriteLine(" 4: Clientes repetidos");
                Console.WriteLine(" 5: Ganancia diaria");


                var menuId = ParseOption();

                if (menuId == -1) continue;

                switch (menuId)
                {
                    case 0: inMenu = false; break;
                    case 1: LookupOrdersUseCase.LookupRecords(allRecords, productCosts); break;
                    case 2: PrintOutSoldInventoryUseCase.PrintOutInventory(allRecords, productCosts); break;
                    case 3: PrintOutCanceledOrdersUseCase.PrintOutInventory(allRecords, productCosts); break;
                    case 4: RepeatCustomersUseCase.PrintOutRepeatCustomers(allRecords, productCosts); break;
                    case 5: PrintOutDailyTotalsUseCase.PrintOutDailyTotals(allRecords, productCosts); break;
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
                Console.WriteLine("No es una opción de menú válida, inténtelo de nuevo.");
                return -1;
            }

            var validIds = new List<long> { 0, 1, 2, 3, 4, 5 };
            if (!validIds.Contains(menuId))
            {
                Console.WriteLine("No ingresaste un número de menú válido");
                return -1;
            }

            return menuId;
        }
    }
}
