using System.Web.Mvc;
using System.Web.Routing;

namespace SDL.RazorMediatorLayouts
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            //http://localhost/Views/ComponentLayouts/FileName.cshtml
            routes.MapRoute(
                name: "GetByComponentLayoutPath",
                url: "ComponentLayouts/{file}",
                defaults: new { controller = "Default", action = "GetByComponentLayoutPath", file = UrlParameter.Optional }
            );

            //http://localhost/Views/PageLayouts/FileName.cshtml
            routes.MapRoute(
                name: "GetByPageLayoutPath",
                url: "PageLayouts/{file}",
                defaults: new { controller = "Default", action = "GetByPageLayoutPath", file = UrlParameter.Optional }
            );

            //http://localhost/FileName/4042-29578/5057-30730-32
            routes.MapRoute(
                name: "GetByUri",
                url: "{file}/{item}/{template}",
                defaults: new { controller = "Default", action = "GetByUri", file = UrlParameter.Optional, item = UrlParameter.Optional, template = UrlParameter.Optional }
            );

        }
    }
}

