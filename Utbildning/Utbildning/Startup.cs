using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Utbildning.Startup))]
namespace Utbildning
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
