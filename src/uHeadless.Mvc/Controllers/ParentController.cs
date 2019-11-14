using System;
using System.Net;
using System.Web.Http;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web.WebApi;

namespace uHeadless.Mvc.Controllers
{
    public class ParentController : UmbracoApiController
    {
        [System.Web.Mvc.HttpGet]
        public IPublishedContent Get(Guid id)
        {
            IPublishedContent content = Umbraco.Content(id).Parent;
            if (content != null)
            {
                return content.Parent;
            }

            throw new HttpResponseException(HttpStatusCode.NotFound);

            return null;
        }
    }
}