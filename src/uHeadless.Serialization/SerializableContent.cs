using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Umbraco.Core.Models.PublishedContent;

namespace uHeadless.Serialization
{
    public class SerializableContent
    {
        List<object> properties = new List<object>();

        [JsonIgnore]
        protected IPublishedContent Content { get; }

        public SerializableContent(IPublishedContent content)
        {
            Content = content;
            Id = content.Key;
            PopulateProperties(content);
        }

        private void PopulateProperties(IPublishedContent content)
        {
            foreach (var property in content.Properties)
            {
                if (property.PropertyType.ClrType == typeof(IPublishedContent))
                {
                    continue;
                }

                properties.Add(property);
            }
        }

        public Guid Id { get; private set; }

        /*public IEnumerable<SerializableContent> Children
        {
            get { return Content.Children; }
        }*/

        public IEnumerable<object> Properties => properties;
    }
}
