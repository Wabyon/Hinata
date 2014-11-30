using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Hinata.Startup))]
namespace Hinata
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}