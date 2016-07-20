using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace TasksWeb
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.MapRoute(
                name: "GetTaskList",
                url: "tasks",
                defaults: new { controller = "Home", action = "GetTaskList" }
            );
            routes.MapRoute(
                name: "GetTaskInfo",
                url: "getTask",
                defaults: new { controller = "Home", action = "GetTaskInfo" }
            );
            routes.MapRoute(
                name: "ExecuteTask",
                url: "executeTask",
                defaults: new { controller = "Home", action = "ExecuteTask" }
            );
            routes.MapRoute(
                name: "QuanShiExecuteTask",
                url: "quanShiExecuteTask",
                defaults: new { controller = "Home", action = "QuanShiExecuteTask" }
            );

            routes.MapRoute(
                name: "GetBusinessData",
                url: "getBusinessData",
                defaults: new { controller = "Home", action = "GetBusinessData" }
            );
            routes.MapRoute(
                name: "Login",
                url: "login",
                defaults: new { controller = "Home", action = "Login" }
            );
            routes.MapRoute("task", "task/{*SN}", new { controller = "Home", action = "Index" });
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
