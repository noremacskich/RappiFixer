using RappiFixer.Extensions;
using RappiFixer.Helpers;
using RappiFixer.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace RappiFixer.UseCases
{
    public class PrintOutHourlyTotalsUseCase
    {

        const int calenderCellWidth = 14;


        internal static void PrintOutHourlyTotals(List<RappiDataModel> allRecords, List<ProductCost> productCosts)
        {

            var calendarDays = allRecords
                .Where(x => x.OrderState == "finished")
                .GroupBy(x => x.CreateDate.Date.DayOfWeek)
                .Select(x => new
                {
                    DOW = x.Key,
                    Count = x.Sum(x => x.NumberOfUnits),
                    Profit = x.ToList().Sum(a => (productCosts.FirstOrDefault(y => y.PROMOCION.Trim().Equals(a.ProductName, StringComparison.InvariantCultureIgnoreCase))?.GANACIA ?? 0) * a.NumberOfUnits),
                    Cost = x.Sum(x => x.Cost),
                    AllOrderItems = x.ToList(),
                    NumberOfOrders = x.GroupBy(x => x.OrderId).Count()
                });

            // Gets the Calendar instance associated with a CultureInfo.
            CultureInfo myCI = CultureInfo.CurrentCulture;
            Calendar myCal = myCI.Calendar;

            // Gets the DTFI properties required by GetWeekOfYear.
            CalendarWeekRule myCWR = myCI.DateTimeFormat.CalendarWeekRule;
            DayOfWeek myFirstDOW = myCI.DateTimeFormat.FirstDayOfWeek;


            var calendar = new CalendarDay[1, 7];


            var calendarWeekDayIndex = 0;
            var calendarWeekIndex = 0;


            foreach(var dayOfWeek in GetDaysOfWeek(myCI))
            {

                var calendarDay = new CalendarDay()
                {
                    DOW = dayOfWeek,
                    WithinDateRange = true,
                    HourlyBreakdown = new List<HourBreakdown>()
                };

                var lookedUpDate = calendarDays.FirstOrDefault(x => x.DOW == dayOfWeek);

                calendarDay.NumberOfOrders = lookedUpDate.NumberOfOrders;
                calendarDay.Profit = lookedUpDate.Profit;
                calendarDay.Cost = lookedUpDate.Cost;

                var convertedHourlyRecords = lookedUpDate.AllOrderItems.GroupBy(x => x.OrderId)
                .Select(x => new
                {
                    originalItem = x,
                    TimeStamp = x.First().CreateDate,
                }).ToList();


                calendarDay.HourlyBreakdown.AddRange(convertedHourlyRecords
                    .OrderBy(x => x.TimeStamp.Hour)
                    .GroupBy(x => x.TimeStamp.Hour)
                .Select(x => new HourBreakdown()
                {
                    HourTimeStamp = x.First().TimeStamp.Hour,
                    HourText = x.First().TimeStamp.ToString("h tt"),
                    ItemCount = x.Count(),
                    Average = x.GroupBy(y => y.TimeStamp.DayOfYear).Count()
                }));

                calendar[calendarWeekIndex, calendarWeekDayIndex] = calendarDay;

                calendarWeekDayIndex++;
            };

            PrintOutCalendar(calendar, 1);

        }

        private class CalendarDay
        {
            public DateTime Day { get; set; }
            public DayOfWeek DOW { get; set; }
            public int NumberOfOrders { get; set; }
            public double Profit { get; set; }
            public double Cost { get; set; }
            public bool WithinDateRange { get; set; }
            public List<HourBreakdown> HourlyBreakdown {get; set; }
        }

        private class HourBreakdown
        {
            public int ItemCount {get; set;}
            public int HourTimeStamp {get; set;}
            public string HourText { get; set; }
            public object Average { get; internal set; }
        }

        private static void PrintOutCalendar(CalendarDay[,] calendarRow, int totalWeeks)
        {
            const int daysInWeek = 7;

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();

            var weekDayHeaders = GetLocalizedDayOfWeekValues(CultureInfo.CurrentCulture);

            for (var day = 0; day < daysInWeek; day++)
            {
                Console.Write($" {weekDayHeaders[day].CenterString(calenderCellWidth)}");
            }

            Console.WriteLine();
            for (var week = 0; week < totalWeeks; week++)
            {
                Console.WriteLine(new String('-', (calenderCellWidth + 1) * daysInWeek + 1));

                for (var day = 0; day < daysInWeek; day++)
                {
                    PrintCell(calendarRow[week, day], $"|{calendarRow[week, day].NumberOfOrders,-calenderCellWidth}");
                }

                Console.WriteLine("|");

                for (var day = 0; day < daysInWeek; day++)
                {
                    PrintCell(calendarRow[week, day], $"|{calendarRow[week, day].Cost,calenderCellWidth:c}");
                }

                Console.WriteLine("|");

                for (var day = 0; day < daysInWeek; day++)
                {
                    PrintCell(calendarRow[week, day], $"|{calendarRow[week, day].Profit,calenderCellWidth:c}");
                }

                Console.WriteLine("|");


                var mostHoursInDay = 0;

                for (var day = 0; day < daysInWeek; day++)
                {
                    if(calendarRow[week, day].HourlyBreakdown.Count > mostHoursInDay){
                        mostHoursInDay = calendarRow[week, day].HourlyBreakdown.Count;
                    }
                }

                for (var hourlyCount = 0; hourlyCount < mostHoursInDay; hourlyCount++){
                    for (var day = 0; day < daysInWeek; day++)
                    {
                        var calendarDay = calendarRow[week, day];
                        if(calendarDay.HourlyBreakdown.Count <= hourlyCount){
                            PrintCell(calendarRow[week, day], $"|{" ",-calenderCellWidth}");
                        }else{
                            var hour = calendarDay.HourlyBreakdown.OrderBy(x => x.HourTimeStamp).ToList()[hourlyCount];
                            PrintCell(calendarRow[week, day], $"|{hour.HourText + ": " + hour.ItemCount + ":" + hour.Average,-calenderCellWidth}");
                        }
                    }
                    Console.WriteLine("|");
                }

            }

            Console.WriteLine(new String('-', (calenderCellWidth + 1) * daysInWeek + 1));

            Console.WriteLine("Cómo leer: Hora: 5 pedidos en 2 días diferentes (5:2)");
        }

        private static void PrintCell(CalendarDay day, string normalText)
        {
            if (day.WithinDateRange)
            {
                Console.Write(normalText);
            }
            else
            {
                Console.Write($"|{" ",calenderCellWidth}");
            }
        }

        public static List<String> GetLocalizedDayOfWeekValues(CultureInfo culture)
        {
            return GetLocalizedDayOfWeekValues(culture, culture.DateTimeFormat.FirstDayOfWeek);
        }

        public static List<String> GetLocalizedDayOfWeekValues(CultureInfo culture, DayOfWeek startDay)
        {
            string[] dayNames = culture.DateTimeFormat.DayNames;
            IEnumerable<string> query = dayNames
                .Skip((int)startDay)
                .Concat(
                    dayNames.Take((int)startDay)
                );

            return query.ToList();
        }

        public static List<DayOfWeek> GetDaysOfWeek(CultureInfo culture)
        {
            var numberOfDaysInWeek = 0;
            var dayOfWeek = new List<DayOfWeek>();
            var startOfWeek = culture.DateTimeFormat.FirstDayOfWeek;
            while(numberOfDaysInWeek < 7) 
            {
                dayOfWeek.Add(startOfWeek);

                startOfWeek++;
                numberOfDaysInWeek++;
                if(startOfWeek > DayOfWeek.Saturday)
                {
                    startOfWeek = DayOfWeek.Sunday;
                }

            }

            return dayOfWeek;
        }

    }
}
