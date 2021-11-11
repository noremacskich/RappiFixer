using RappiFixer.Extensions;
using RappiFixer.Helpers;
using RappiFixer.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace RappiFixer.UseCases
{
    public class PrintOutMonthlyTotalsUseCase
    {

        internal static void PrintOutMonthlyTotals(List<CSVHeaders> allRecords, List<ProductCost> productCosts)
        {

            var calendarDays = allRecords
                .Where(x => x.state == "finished")
                .GroupBy(x => DateTime.Parse(x.created_at.Substring(0, 10)).Date)
                .Select(x => new
                {
                    OrderDate = x.Key,
                    Count = x.Sum(x => x.product_units),
                    Profit = (productCosts.FirstOrDefault(y => y.PROMOCION.Trim().Equals(x.First().product, StringComparison.InvariantCultureIgnoreCase))?.GANACIA ?? 0) * x.Sum(x => x.product_units),
                    Cost = x.Sum(x => x.product_total_price_with_discount),
                    AllOrderItems = x.ToList(),
                    NumberOfOrders = x.GroupBy(x => x.order_id).Count()
                });

            // Gets the Calendar instance associated with a CultureInfo.
            CultureInfo myCI = CultureInfo.CurrentCulture;
            Calendar myCal = myCI.Calendar;

            // Gets the DTFI properties required by GetWeekOfYear.
            CalendarWeekRule myCWR = myCI.DateTimeFormat.CalendarWeekRule;
            DayOfWeek myFirstDOW = myCI.DateTimeFormat.FirstDayOfWeek;

            var startDate = calendarDays.Min(x => x.OrderDate);
            var endDate = calendarDays.Max(x => x.OrderDate);

            startDate = startDate.StartOfWeek(myFirstDOW);
            endDate = endDate.StartOfWeek(myFirstDOW).AddDays(7);

            var numberOfCalendarWeeks = (myCal.GetWeekOfYear(endDate, myCWR, myFirstDOW) - myCal.GetWeekOfYear(startDate, myCWR, myFirstDOW)) + 1;

            var calendar = new CalendarDay[numberOfCalendarWeeks, 7];


            var calendarWeekDayIndex = 0;
            var calendarWeekIndex = 0;

            while (startDate <= endDate)
            {
                var calendarDay = new CalendarDay()
                {
                    Day = startDate
                };

                var lookedUpDate = calendarDays.FirstOrDefault(x => x.OrderDate.Date == startDate.Date);

                if (lookedUpDate != null)
                {
                    calendarDay.NumberOfOrders = lookedUpDate.NumberOfOrders;
                    calendarDay.Profit = lookedUpDate.Profit;
                    calendarDay.Cost = lookedUpDate.Cost;
                }

                calendar[calendarWeekIndex, calendarWeekDayIndex] = calendarDay;
                
                calendarWeekDayIndex++;

                

                if(calendarWeekDayIndex % 7 == 0 && startDate != endDate)
                {
                    calendarWeekDayIndex = 0;
                    calendarWeekIndex++;
                }

                startDate = startDate.AddDays(1);
            }


            PrintOutCalendar(calendar, calendarWeekIndex);



        }

        private class CalendarDay
        {
            public DateTime Day { get; set; }
            public int NumberOfOrders { get; set; }
            public double Profit { get; set; }
            public double Cost { get; set; }
        }

        private static void PrintOutCalendar(CalendarDay[,] calendarRow, int totalWeeks)
        {
            const int calenderCellWidth = 14;
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

                for(var day = 0; day < daysInWeek; day++)
                {
                    Console.Write($"|{calendarRow[week, day].Day.Day.ToString().CenterString(calenderCellWidth)}");
                }

                Console.WriteLine("|");

                Console.WriteLine(new String('-', (calenderCellWidth + 1) * daysInWeek + 1));

                for (var day = 0; day < daysInWeek; day++)
                {
                    Console.Write($"|{calendarRow[week, day].NumberOfOrders,-calenderCellWidth}");
                }

                Console.WriteLine("|");

                for (var day = 0; day < daysInWeek; day++)
                {
                    Console.Write($"|{calendarRow[week, day].Cost,calenderCellWidth:c}");
                }

                Console.WriteLine("|");

                for (var day = 0; day < daysInWeek; day++)
                {
                    Console.Write($"|{calendarRow[week, day].Profit,calenderCellWidth:c}");
                }

                Console.WriteLine("|");

            }

            Console.WriteLine(new String('-', (calenderCellWidth + 1) * daysInWeek + 1));


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

    }
}
