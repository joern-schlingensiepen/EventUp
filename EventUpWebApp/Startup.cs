using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(EventUpWebApp.Startup))]
namespace EventUpWebApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
