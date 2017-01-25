using Microsoft.Owin;
using Owin;
using SMARTchatWEB;

[assembly: OwinStartup(typeof(Startup))]
namespace SMARTchatWEB
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            app.MapSignalR();
        }
    }
}
