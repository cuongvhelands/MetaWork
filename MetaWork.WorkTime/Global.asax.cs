using MetaWork.WorkTime.Controllers;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace MetaWork.WorkTime
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            var conn = System.Configuration.ConfigurationManager.ConnectionStrings["Data.Properties.Settings.TCTTimerConnectionString"];
            if (conn != null) Data.Provider.Main.AdminConnStr = conn.ConnectionString;
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            HomeController home = new HomeController();
            home.ServiceAuto();
            home.AddAuthenticationFolder();
            //GlobalHost.DependencyResolver.Register(typeof(IUserIdProvider), () => new MyIdProvider());
        }
    }
}
