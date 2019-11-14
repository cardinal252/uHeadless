using System;
using System.Linq;
using System.Net;
using System.Web.Http;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web.WebApi;

namespace uHeadless.Mvc.Controllers
{
    public class ChildrenController : UmbracoApiController
    {
        [System.Web.Mvc.HttpGet]
        public IPublishedContent[] Get(Guid id)
        {
            IPublishedContent content = Umbraco.Content(id);
            if (content != null)
            {
                return content.Children.ToArray();
            }

            throw new HttpResponseException(HttpStatusCode.NotFound);
        }
    }
}