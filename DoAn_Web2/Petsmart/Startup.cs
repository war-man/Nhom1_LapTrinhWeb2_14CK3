using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Petsmart.Startup))]
namespace Petsmart
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
