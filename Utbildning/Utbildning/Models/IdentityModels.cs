using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Utbildning.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public string ProfilePicture { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public System.Data.Entity.DbSet<Utbildning.Models.Booking> Bookings { get; set; }

        public System.Data.Entity.DbSet<Utbildning.Models.CourseOccasion> CourseOccasions { get; set; }

        public System.Data.Entity.DbSet<Utbildning.Models.BulletPoints> BulletPoints { get; set; }

        public System.Data.Entity.DbSet<Utbildning.Models.Course> Courses { get; set; }
        
        public System.Data.Entity.DbSet<Utbildning.Models.Log> Logs { get; set; }

        public System.Data.Entity.DbSet<Utbildning.Models.BookingData> BookingDatas { get; set; }

        public System.Data.Entity.DbSet<Utbildning.Models.CourseTags> CourseTags { get; set; }
    }
}