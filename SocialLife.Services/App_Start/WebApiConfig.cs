using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace SocialLife.Services
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "UsersApi",
                routeTemplate: "api/users/{action}/{id}",
                defaults: new { controller = "users", id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "ProfilesApi",
                routeTemplate: "api/profiles/{action}/{id}",
                defaults: new { controller = "profiles", id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "MessagesApi",
                routeTemplate: "api/messages/{action}/{id}",
                defaults: new { controller = "messages", id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "EventsApi",
                routeTemplate: "api/events/{action}/{id}",
                defaults: new { controller = "events", id = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
                name: "WakeApi",
                routeTemplate: "api/wake/{action}",
                defaults: new { controller = "wake" }
            );

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            // Uncomment the following line of code to enable query support for actions with an IQueryable or IQueryable<T> return type.
            // To avoid processing unexpected or malicious queries, use the validation settings on QueryableAttribute to validate incoming queries.
            // For more information, visit http://go.microsoft.com/fwlink/?LinkId=279712.
            //config.EnableQuerySupport();

            // To disable tracing in your application, please comment out or remove the following line of code
            // For more information, refer to: http://www.asp.net/web-api
            config.EnableSystemDiagnosticsTracing();
        }
    }
}
