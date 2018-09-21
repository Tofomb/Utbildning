using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;
using Utbildning.Models;

[assembly: OwinStartupAttribute(typeof(Utbildning.Startup))]
namespace Utbildning
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
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
    }
}
