using System.Web.Http;
using uHeadless.Serialization;
using Umbraco.Core.Composing;
using Umbraco.Web.Runtime;

namespace uHeadless.Mvc
{
    [ComposeAfter(typeof(WebFinalComposer))]
    public class WebApiComposer : ComponentComposer<WebApiComponent>
    {
    }

    public class WebApiComponent : IComponent
    {
        public void Initialize()
        {
            GlobalConfiguration.Configuration.Routes.MapHttpRoute(
                name: "ContentByIdRoute",
                routeTemplate: "headless/content/id/{id}",
                defaults: new { controller = "ContentById" }
            );
            GlobalConfiguration.Configuration.Routes.MapHttpRoute(
                name: "ParentRoute",
                routeTemplate: "headless/parent/{id}",
                defaults: new { controller = "Parent" }
            );
            GlobalConfiguration.Configuration.Routes.MapHttpRoute(
                name: "ContentByUrlRoute",
                routeTemplate: "headless/content/url",
                defaults: new { controller = "ContentByUrl" }
            );
            GlobalConfiguration.Configuration.Routes.MapHttpRoute(
                name: "ChildrenRoute",
                routeTemplate: "headless/children/{id}",
                defaults: new { controller = "Children" }
            );
            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.Converters.Add(new PublishedContentJsonConverter());
            GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.Converters.Add(new HtmlStringConverter());
        }

        public void Terminate()
        {
            throw new System.NotImplementedException();
        }
    }
}