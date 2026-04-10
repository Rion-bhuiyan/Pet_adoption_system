using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(pet_adoption_system.Startup))]
namespace pet_adoption_system
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
