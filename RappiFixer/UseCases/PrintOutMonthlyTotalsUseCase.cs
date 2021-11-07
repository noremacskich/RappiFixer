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


            var startDate = calendarDays.Min(x => x.OrderDate);
            var endDate = calendarDays.Max(x => x.OrderDate);

            // Gets the Calendar instance associated with a CultureInfo.
            CultureInfo myCI = CultureInfo.CurrentCulture;
            Calendar myCal = myCI.Calendar;

            // Gets the DTFI properties required by GetWeekOfYear.
            CalendarWeekRule myCWR = myCI.DateTimeFormat.CalendarWeekRule;
            DayOfWeek myFirstDOW = myCI.DateTimeFormat.FirstDayOfWeek;


            var numberOfCalendarWeeks = (myCal.GetWeekOfYear(endDate, myCWR, myFirstDOW) - myCal.GetWeekOfYear(startDate, myCWR, myFirstDOW)) + 1;

            var calendar = new CalendarDay[numberOfCalendarWeeks, 7];


            var firstDayInFirstWeek = startDate.StartOfWeek(myFirstDOW);


            var calendarWeekDayIndex = 0;
            var calendarWeekIndex = 0;

            while (firstDayInFirstWeek <= endDate)
            {
                var calendarDay = new CalendarDay()
                {
                    Day = firstDayInFirstWeek
                };


                var lookedUpDate = calendarDays.FirstOrDefault(x => x.OrderDate.Date == firstDayInFirstWeek.Date);

                if (lookedUpDate != null)
                {
                    calendarDay.NumberOfOrders = lookedUpDate.NumberOfOrders;
                    calendarDay.Profit = lookedUpDate.Profit;
                    calendarDay.Cost = lookedUpDate.Cost;
                }


                calendar[calendarWeekIndex, calendarWeekDayIndex] = calendarDay;

                calendarWeekDayIndex++;

                if(calendarWeekDayIndex % 7 == 0)
                {
                    calendarWeekDayIndex = 0;
                    calendarWeekIndex++;
                }

                firstDayInFirstWeek = firstDayInFirstWeek.AddDays(1);
            }

            var goo = 1;

            //while (startDate <= endDate)
            //{
            //    var date = startDate.ToLongDateString();
            //    var numberOfOrders = 0;
            //    double profit = 0;
            //    double cost = 0;

            //    var lookedUpDate = calendarDays.FirstOrDefault(x => x.OrderDate.Date == startDate.Date);

            //    if (lookedUpDate != null)
            //    {
            //        numberOfOrders = lookedUpDate.NumberOfOrders;
            //        profit = lookedUpDate.Profit;
            //        cost = lookedUpDate.Cost;
            //    }

            //    Console.WriteLine($"Date : {date}");
            //    Console.WriteLine($"Number Of Orders : {numberOfOrders}");
            //    Console.WriteLine($"Cost : {cost:c}");
            //    Console.WriteLine($"Profit : {profit:c}");
            //    Console.WriteLine();
            //    Console.WriteLine();

            //    startDate = startDate.AddDays(1);
            //}



        }

        private class CalendarDay
        {
            public DateTime Day { get; set; }
            public int NumberOfOrders { get; set; }
            public double Profit { get; set; }
            public double Cost { get; set; }
        }

    }
}
