using RappiFixer.Extensions;
using RappiFixer.Helpers;
using RappiFixer.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace RappiFixer.UseCases
{
    public class PrintOutHourlyMonthlyTotalsUseCase
    {

        const int calenderCellWidth = 14;


        internal static void PrintOutHourlyMonthlyTotals(List<CSVHeaders> allRecords, List<ProductCost> productCosts)
        {

            var calendarDays = allRecords
                .Where(x => x.state == "finished")
                .GroupBy(x => DateTime.Parse(x.created_at.Substring(0, 10)).Date)
                .Select(x => new
                {
                    OrderDate = x.Key,
                    Count = x.Sum(x => x.product_units),
                    Profit = x.ToList().Sum(a => (productCosts.FirstOrDefault(y => y.PROMOCION.Trim().Equals(a.product, StringComparison.InvariantCultureIgnoreCase))?.GANACIA ?? 0) * a.product_units),
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


            var originalStartDate = calendarDays.Min(x => x.OrderDate);
            var originalEndDate = calendarDays.Max(x => x.OrderDate);


            var startDate = originalStartDate.StartOfWeek(myFirstDOW);
            var endDate = originalEndDate.StartOfWeek(myFirstDOW).AddDays(7);

            var numberOfCalendarWeeks = (myCal.GetWeekOfYear(endDate, myCWR, myFirstDOW) - myCal.GetWeekOfYear(startDate, myCWR, myFirstDOW)) + 1;

            var calendar = new CalendarDay[numberOfCalendarWeeks, 7];


            var calendarWeekDayIndex = 0;
            var calendarWeekIndex = 0;

            while (startDate <= endDate)
            {
                var calendarDay = new CalendarDay()
                {
                    Day = startDate,
                    WithinDateRange = startDate >= originalStartDate && startDate <= originalEndDate,
                    HourlyBreakdown = new List<HourBreakdown>()
                };

                var lookedUpDate = calendarDays.FirstOrDefault(x => x.OrderDate.Date == startDate.Date);

                if (lookedUpDate != null)
                {
                    calendarDay.NumberOfOrders = lookedUpDate.NumberOfOrders;
                    calendarDay.Profit = lookedUpDate.Profit;
                    calendarDay.Cost = lookedUpDate.Cost;

                    var convertedHourlyRecords = lookedUpDate.AllOrderItems
                    .Select(x => new {
                        originalItem = x,
                        TimeStamp = DateTime.ParseExact(x.created_at.Substring(0, 19), "yyyy-MM-dd HH:mm:ss", myCI.DateTimeFormat),
                        HourInteger = DateTime.ParseExact(x.created_at.Substring(0, 19), "yyyy-MM-dd HH:mm:ss", myCI.DateTimeFormat).Hour,
                    }).ToList();


                    calendarDay.HourlyBreakdown.AddRange(convertedHourlyRecords.GroupBy(x => x.HourInteger)
                    .Select(x => new HourBreakdown(){
                            TimeStamp = x.First().TimeStamp,
                            HourText = x.First().TimeStamp.ToString("H tt"),
                            ItemCount = x.Count()
                        }).ToList());
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
            public bool WithinDateRange { get; set; }
            public List<HourBreakdown> HourlyBreakdown {get; set; }
        }

        private class HourBreakdown
        {
            public int ItemCount {get; set;}
            public DateTime TimeStamp {get; set;}
            public string HourText { get; set; }
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

                for(var day = 0; day < daysInWeek; day++)
                {
                    Console.Write($"|{calendarRow[week, day].Day.Day.ToString().CenterString(calenderCellWidth)}");
                }

                Console.WriteLine("|");

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
                            var hour = calendarDay.HourlyBreakdown[hourlyCount];
                            PrintCell(calendarRow[week, day], $"|{hour.HourText + ": " + hour.ItemCount,-calenderCellWidth}");
                        }
                    }
                    Console.WriteLine("|");
                }

            }

            Console.WriteLine(new String('-', (calenderCellWidth + 1) * daysInWeek + 1));


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

    }
}
