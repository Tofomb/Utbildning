using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Utbildning.Models;

namespace Utbildning.Classes
{
    public static class MailHandler
    {
        public static string GetEmailsString(this CourseOccasion co)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                List<Booking> bookings = db.Bookings.Where(x => x.Email == co.GetCourse().Email).ToList();
                List<string> emails = (from b in bookings
                                       select b.Email).ToList();
                return emails.Aggregate((x, y) => x + ", " + y);
            }
        }

        public static string GenMailTo(string Subject = null, string Body = null, string Recipient = null, string BCC = null)
        {
            return "";
            //return "mailto:" + Recipient + "?" + (Subject != null ? "" + Subject : "")
        }
    }
}