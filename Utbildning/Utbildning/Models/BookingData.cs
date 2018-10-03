using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Utbildning.Models
{
    public class BookingData
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public string Company { get; set; }
        public string BillingCity { get; set; }
        public string BillingPostalCode { get; set; }
        public int Bookings { get; set; }
        public string DiscountCode { get; set; }
        public DateTime BookingDate { get; set; }
        public int Course { get; set; }
        public int CourseOccasion { get; set; }
        public string CourseName { get; set; }
        public string Host { get; set; } // = AltHost == null ? Course.Host ID : "[ALT]"
        public string CourseCity { get; set; } // = AltCity ?? Course.City
        public string CourseAddress { get; set; } // = AltAddress ?? Address;
        public int MinPeople { get; set; }
        public int MaxPeople { get; set; }
        public DateTime StartDate { get; set; }
        public int CourseLength { get; set; }
        public float Price { get; set; }        

    }
}