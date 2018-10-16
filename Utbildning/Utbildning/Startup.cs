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
        private const int ExpirationTime = 30; //Amount of days program waits before deleting courseoccasions

        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            StartDBThread();
            CreateRolesAndDefaultUsers();
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

                var AdminUser = new ApplicationUser() { UserName = "admin@castra.se" };
                string Pw = "Q2FzdHJh";

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

                //REMOVE FOLLOWING LATER WHEN OBSOLETE. Only used to have a default KL user.
                var KLUser = new ApplicationUser() { UserName = "kl@m.m" };
                string Pw = "Q2FzdHJh";

                var NewUser = UserMng.Create(KLUser, Pw);

                if (NewUser.Succeeded)
                {
                    var result = UserMng.AddToRole(KLUser.Id, "Kursledare");
                }
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
                List<CourseOccasion> OldCOs = db.CourseOccasions.ToList().Where(x => x.StartDate.AddDays(ExpirationTime) < DateTime.Now).ToList();
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
