using System.Net;
using System.Web.Http;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web.WebApi;

namespace uHeadless.Mvc.Controllers
{
    public class ContentByUrlController : UmbracoApiController
    {
        [System.Web.Mvc.HttpPost]
        public IPublishedContent Post([FromBody]string url)
        {
            IPublishedContent content = UmbracoContextAccessor.UmbracoContext.Content.GetByRoute(url);
            if (content != null)
            {
                return content;
            }

            throw new HttpResponseException(HttpStatusCode.NotFound);
        }
    }
}