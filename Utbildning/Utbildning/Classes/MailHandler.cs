using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Web;
using Utbildning.Models;

namespace Utbildning.Classes
{
    public static class MailHandler
    {
        public static string GetEmailsString(this CourseOccasion co, bool SemiColon = false)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                List<Booking> bookings = db.Bookings.Where(x => x.Email == co.GetCourse().Email).ToList();
                List<string> emails = (from b in bookings
                                       select b.Email).ToList();
                return emails.Aggregate((x, y) => x + (SemiColon ? "; " : ", ") + y);
            }
        }

        public static void Send(string Recipient, string Subject, string Body)
        {
            MailMessage mail = new MailMessage("CASTRA MAIL ???", Recipient);
            SmtpClient client = new SmtpClient
            {
                Port = 25, //?
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Host = "" //?
            };

            mail.Subject = Subject;
            mail.Body = Body;
            client.Send(mail);
        }

        public static void Send(string[] Recipients, string Subject, string Body)
        {
            MailMessage mail = new MailMessage("CASTRA MAIL ???", "");
            foreach (string s in Recipients)
            {
                mail.Bcc.Add(s);
            }
            SmtpClient client = new SmtpClient
            {
                Port = 25, //?
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Host = "" //?
            };

            mail.Subject = Subject;
            mail.Body = Body;
            client.Send(mail);
        }

        public static void SendTester(string From, string Recipient, string Subject, string Body, string Password)
        {
            MailMessage mail = new MailMessage(From, Recipient);
            SmtpClient client = new SmtpClient("smtp.gmail.com", 587)
            {
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                EnableSsl = true,
                Credentials = new NetworkCredential(From, Password)
            };

            mail.Subject = Subject;
            mail.Body = Body;
            client.Send(mail);
        }

        public static string GetInfo(string Email)
        {
            List<Booking> bookings;
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                bookings = db.Bookings.ToList().Where(x => x.Email == Email).ToList();
            }

            string s = "";

            foreach (var x in bookings)
            {
                PropertyInfo[] properties = x.GetType().GetProperties();
                foreach(var y in properties)
                {
                    s += $"{y.Name}: {y.GetValue(x, null)}<br/>";
                }
            }
            return s;
        }
    }
}