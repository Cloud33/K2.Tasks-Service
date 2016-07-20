using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using Microsoft.AspNet.SignalR;

[assembly: OwinStartup(typeof(TasksWeb.Startup))]
[assembly: log4net.Config.XmlConfigurator(ConfigFile = "Web.config", Watch = true)]
namespace TasksWeb
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // 有关如何配置应用程序的详细信息，请访问 http://go.microsoft.com/fwlink/?LinkID=316888
            app.MapSignalR();
        }
    }
}
