using System;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace uHeadless.Serialization
{
    public class HtmlStringConverter : JsonConverter
    {

        public override bool CanConvert(Type objectType)
        {
            return typeof(IHtmlString).IsAssignableFrom(objectType);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            HtmlString htmlString = value as HtmlString;
            if (htmlString == null)
            {
                return;
            }

            writer.WriteValue(htmlString.ToString());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            string value = reader.Value as string;

            if (objectType == typeof(MvcHtmlString))
            {
                return new MvcHtmlString(value);
            }

            if (objectType == typeof(HtmlString))
            {
                return new HtmlString(value);
            }

            throw new NotImplementedException("This is an unknown implementation of the html string");
        }
    }
}