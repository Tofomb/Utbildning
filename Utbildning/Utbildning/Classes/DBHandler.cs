using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using Utbildning.Models;

namespace Utbildning.Classes
{
    public static class DBHandler
    {
        private static ApplicationDbContext db = new ApplicationDbContext();

        public static int GetBookings(this CourseOccasion co)
        {
            List<Booking> bookings = db.Bookings.ToList();
            return bookings.Where(x => x.CourseOccasionId == co.Id).Sum(y => y.Bookings);
        }

        public static int GetAvailableBookings(this CourseOccasion co) => co.MaxPeople - co.GetBookings();

        public static bool EnoughAvailable(this CourseOccasion co, int NewBookings) => co.GetAvailableBookings() - NewBookings >= 0;

        public static bool EnoughBookings(this CourseOccasion co) => co.GetBookings() >= co.MinPeople;

        public static string Format(this DateTime dt)
        {
            string dtString = dt.ToString("dddd, dd MMMM yyyy", new CultureInfo("sv-SE"));
            if (dtString.Length > 0)
                return char.ToUpper(dtString[0]) + dtString.Substring(1);
            return "Failed to load.";
        }
    }
}