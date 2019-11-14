namespace Umbraco.Serialization
{
    using System;
    using System.Linq;
    using System.Web;
    using Core.Models.PublishedContent;
    using Newtonsoft.Json;

    public class ImageSerializer : JsonConverter<>
    {
        private static string[] allowedProperties = {"name", "url"};

        private static string[] mediaProperties = { "UmbracoBytes", "UmbracoExtension", "UmbracoFile" };

        private static string[] publishedContentProperties => typeof(PublishedContentWrapped).GetProperties().Where(x => !allowedProperties.Contains(x.Name.ToLowerInvariant())).Select(x => x.Name).ToArray();

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (!(value is IPublishedContent content))
            {
                return;
            }

            writer.WriteStartObject();

            WriteCoreInformation(writer, content);

            WriteGeneratedProperties(writer, serializer, content);

            //WriteHeirachyInformation(writer, content);

            writer.WriteEndObject();
        }

        private static void WriteCoreInformation(JsonWriter writer, IPublishedContent content)
        {
            writer.WritePropertyName("id");
            writer.WriteValue(content.Key);
        }

        protected void WriteHeirachyInformation(JsonWriter writer, IPublishedContent content)
        {
            if (content.ItemType == PublishedItemType.Media)
            {
                return;
            }

            var serialiseMetadata = content.GetProperty("showMetadata");
            if (serialiseMetadata?.GetValue().ToString() == "0")
            {
                return;
            }

            writer.WritePropertyName("hasChildren");
            writer.WriteValue(content.Children?.Any());
            writer.WritePropertyName("hasParent");
            writer.WriteValue(content.Parent != null);
        }

        private void WriteGeneratedProperties(JsonWriter writer, JsonSerializer serializer, IPublishedContent content)
        {
            foreach (var property in content.Properties)
            {
                if (publishedContentProperties.Contains(property.Alias))
                {
                    continue;
                }

                if (content.ItemType == PublishedItemType.Media && mediaProperties.Contains(property.Alias))
                {
                    continue;
                }

                writer.WritePropertyName(ToCamelCase(property.Alias));
                serializer.Serialize(writer, property.GetValue());
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(IPublishedContent).IsAssignableFrom(objectType);
        }

        /// <summary>
        /// Borrowed straight from Newtonsoft JSON.net
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        protected virtual string ToCamelCase(string s)
        {
            if (string.IsNullOrEmpty(s) || !char.IsUpper(s[0]))
            {
                return s;
            }

            char[] chars = s.ToCharArray();

            for (int i = 0; i < chars.Length; i++)
            {
                if (i == 1 && !char.IsUpper(chars[i]))
                {
                    break;
                }

                bool hasNext = (i + 1 < chars.Length);
                if (i > 0 && hasNext && !char.IsUpper(chars[i + 1]))
                {
                    // if the next character is a space, which is not considered uppercase 
                    // (otherwise we wouldn't be here...)
                    // we want to ensure that the following:
                    // 'FOO bar' is rewritten as 'foo bar', and not as 'foO bar'
                    // The code was written in such a way that the first word in uppercase
                    // ends when if finds an uppercase letter followed by a lowercase letter.
                    // now a ' ' (space, (char)32) is considered not upper
                    // but in that case we still want our current character to become lowercase
                    if (char.IsSeparator(chars[i + 1]))
                    {
                        chars[i] = char.ToLowerInvariant(chars[i]);
                    }

                    break;
                }

                chars[i] = char.ToLowerInvariant(chars[i]);
            }

            return new string(chars);
        }

    }
}