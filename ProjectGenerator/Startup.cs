using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ProjectGenerator.Startup))]
namespace ProjectGenerator
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
