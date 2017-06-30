using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using System.Web.Mvc;
using System.Web.Routing;

namespace Episerver_Playground.Business.Plugins.Admin.ListPagesByPageTypes
{
    [InitializableModule]
    public class CustomRouteInitialization : IInitializableModule
    {
        public void Initialize(InitializationEngine context)
        {
            RouteTable.Routes.MapRoute("Default",
                "custom-plugins/list-pages-by-page-types/{action}/{id}/{page}",
                new { controller = "ListPagesByPageTypes", action = "Index", id = UrlParameter.Optional, page = UrlParameter.Optional });
        }

        public void Uninitialize(InitializationEngine context)
        {
        
        }
    }
}