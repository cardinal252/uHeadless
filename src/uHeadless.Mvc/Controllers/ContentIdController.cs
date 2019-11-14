using System;
using System.Net;
using System.Web.Http;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web.WebApi;

namespace uHeadless.Mvc.Controllers
{
    public class ContentByIdController : UmbracoApiController
    {
        [System.Web.Mvc.HttpGet]
        public IPublishedContent Get(Guid id)
        {
            IPublishedContent content = Umbraco.Content(id);
            //SerializableContent serializableContent = new SerializableContent(content);
            if (content != null)
            {
                return content;
            }

            throw new HttpResponseException(HttpStatusCode.NotFound);
        }
    }
}