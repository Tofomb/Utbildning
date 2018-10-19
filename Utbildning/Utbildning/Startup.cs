using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Utbildning.Classes;
using Utbildning.Models;

[assembly: OwinStartupAttribute(typeof(Utbildning.Startup))]
namespace Utbildning
{
    public partial class Startup
    {
        //#Ändra lite grejer här innan den körs första gången
        private const string AdminEmail = "PUT ADMIN MAIL HERE"; //Byt ut mot önskat admin inloggnigns email.
        private const string AdminPW = "PUT ADMIN LOG IN PASSWORD HERE"; //Byt ut mot önskat standard lösenord. Det här lösenordet bör bytas så fort du loggat in första gången.
        private const string ExpirationTime = "60"; //Amount of days program waits before deleting courseoccasions
        private const string Email = "PUT SENDER EMAIL HERE";
        private const string Host = "PUT HOST HERE";
        private const string Port = "PUT PORT HERE";
        private const string Credentials = "PUT EMAIL PASSWORD HERE";

        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            StartDBThread();
            CreateRolesAndDefaultUsers();
            CreateDefaultConfigs();
        }

        private void CreateRolesAndDefaultUsers()
        {
            ApplicationDbContext context = new ApplicationDbContext();

            var RoleMng = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var UserMng = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            if (!RoleMng.RoleExists("Admin"))
            {
                var AdminRole = new IdentityRole() { Name = "Admin" };
                RoleMng.Create(AdminRole);

                var AdminUser = new ApplicationUser() { UserName = AdminEmail, Email = AdminEmail };
                string Pw = AdminPW;

                var NewUser = UserMng.Create(AdminUser, Pw);

                if (NewUser.Succeeded)
                {
                    var result = UserMng.AddToRole(AdminUser.Id, "Admin");
                }
            }

            if (!RoleMng.RoleExists("Kursledare"))
            {
                var KLRole = new IdentityRole() { Name = "Kursledare" };
                RoleMng.Create(KLRole);
            }
        }

        private void CreateDefaultConfigs()
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                List<SiteConfiguration> siteConfigurations = db.SiteConfigurations.ToList();                
                if (!siteConfigurations.Exists(x => x.Property == "ContactTitle"))
                {
                    db.SiteConfigurations.Add(new SiteConfiguration("ContactTitle", "Kontakt"));
                }
                if (!siteConfigurations.Exists(x => x.Property == "ContactText"))
                {
                    db.SiteConfigurations.Add(new SiteConfiguration("ContactText", "Här har du kontaktinfomration om du har några frågor eller annat."));
                }
                if (!siteConfigurations.Exists(x => x.Property == "ContactCompanyName"))
                {
                    db.SiteConfigurations.Add(new SiteConfiguration("ContactCompanyName", "Castra Väst"));
                }
                if (!siteConfigurations.Exists(x => x.Property == "ContactAddress"))
                {
                    db.SiteConfigurations.Add(new SiteConfiguration("ContactAddress", "Engelbrektsgatan 28, 41319 Göteborg"));
                }
                if (!siteConfigurations.Exists(x => x.Property == "ContactPhoneNumber"))
                {
                    db.SiteConfigurations.Add(new SiteConfiguration("ContactPhoneNumber", "031-10 78 10"));
                }
                if (!siteConfigurations.Exists(x => x.Property == "ContactEmail"))
                {
                    db.SiteConfigurations.Add(new SiteConfiguration("ContactEmail", "info@castra.se"));
                }
                if (!siteConfigurations.Exists(x => x.Property == "AboutTitle"))
                {
                    db.SiteConfigurations.Add(new SiteConfiguration("AboutTitle", "Om oss"));
                }
                if (!siteConfigurations.Exists(x => x.Property == "AboutBold"))
                {
                    db.SiteConfigurations.Add(new SiteConfiguration("AboutBold", "Information om Castra Utbildning"));
                }
                if (!siteConfigurations.Exists(x => x.Property == "AboutText"))
                {
                    db.SiteConfigurations.Add(new SiteConfiguration("AboutText", "Castra Utbildning är en sida för dig som vill hitta en kurs för dig eller ditt företag. Här har vi ett stort utbud av kurser, men om det är någon kurs som saknas så får ni gärna höra av er, så kan vi nog anordna något."));
                }
                if (!siteConfigurations.Exists(x => x.Property == "ExpirationTime"))
                {
                    db.SiteConfigurations.Add(new SiteConfiguration("ExpirationTime", ExpirationTime));
                }
                if (!siteConfigurations.Exists(x => x.Property == "Email"))
                {
                    db.SiteConfigurations.Add(new SiteConfiguration("Email", Email));
                }
                if (!siteConfigurations.Exists(x => x.Property == "Host"))
                {
                    db.SiteConfigurations.Add(new SiteConfiguration("Host", Host));
                }
                if (!siteConfigurations.Exists(x => x.Property == "Port"))
                {
                    db.SiteConfigurations.Add(new SiteConfiguration("Port", Port));
                }
                if (!siteConfigurations.Exists(x => x.Property == "Credentials"))
                {
                    db.SiteConfigurations.Add(new SiteConfiguration("Credentials", Credentials));
                }
                db.SaveChanges();
            }
        }

        private void StartDBThread()
        {
            DBInterval();
            System.Threading.Thread t = new System.Threading.Thread(DBThread);
            t.Start();
        }
        private void DBThread()
        {
            Timer timer = new Timer(1000 * 60 * 60 * 3); //Every third hour
            timer.Elapsed += new ElapsedEventHandler(DBInterval);
            timer.AutoReset = true;
            timer.Start();
        }

        private void DBInterval(object sender = null, ElapsedEventArgs e = null)
        {
            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                //ExpirationTime = db.SiteConfiguration.ToList().First().ExpirationTime;
                List<CourseOccasion> OldCOs = db.CourseOccasions.ToList().Where(x => x.StartDate.AddDays(int.Parse(db.SiteConfigurations.Where(y => y.Property == "ExpirationTime").First().Value)) < DateTime.Now).ToList();
                foreach (CourseOccasion co in OldCOs)
                {
                    db.CourseOccasions.Remove(co);
                }
                List<Booking> DoneBookings = db.Bookings.ToList().Where(x => x.GetCourseOccasion().StartDate < DateTime.Now).ToList();
                foreach (Booking b in DoneBookings)
                {
                    var BD = db.BookingDatas.ToList().Where(x => x.BookingId == b.Id).First();
                    if (BD.Status == "OK")
                    {
                        BD.Status = "Klar";
                        db.Entry(BD).State = System.Data.Entity.EntityState.Modified;
                    }
                }
                db.SaveChanges();
            }
        }
    }
}
