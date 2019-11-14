using System;
using System.Linq;
using Newtonsoft.Json;
using Umbraco.Core.Models.PublishedContent;

namespace uHeadless.Serialization
{
    public class PublishedContentJsonConverter : JsonConverter<IPublishedContent>
    {
        protected string[] ExcludedContentProperties { get; }

        private string[] ExcludedMediaProperties { get; }

        public PublishedContentJsonConverter() : this(new string[0], new string[0])
        {
        }

        public PublishedContentJsonConverter(string[] excludedContentProperties, string[] excludedMediaProperties)
        {
            ExcludedContentProperties = excludedContentProperties.Select(x => x.ToLower()).ToArray();
            ExcludedMediaProperties = excludedMediaProperties.Select(x => x.ToLower()).ToArray();
        }

        public override void WriteJson(JsonWriter writer, IPublishedContent content, JsonSerializer serializer)
        {
            writer.WriteStartObject();

            WriteCoreInformation(writer, content);

            switch (content.ItemType)
            {
                case PublishedItemType.Media:
                    WriteMediaProperties(writer, serializer, content);
                    WriteMediaHeirachyInformation(writer, content);
                    break;
                case PublishedItemType.Member:
                case PublishedItemType.Unknown:
                case PublishedItemType.Element:
                    // not sure what we might need here
                    break;
                
                default:
                    WriteContentHeirachyInformation(writer, content);
                    WriteContentProperties(writer, serializer, content);
                    break;
            }

            writer.WriteEndObject();
        }

        protected virtual void WriteCoreInformation(JsonWriter writer, IPublishedContent content)
        {
            writer.WritePropertyName("id");
            writer.WriteValue(content.Key);
        }

        protected virtual void WriteMediaProperties(JsonWriter writer, JsonSerializer serializer, IPublishedContent content)
        {
            foreach (var property in content.Properties)
            {
                string loweredPropertyAlias = property.Alias.ToLower();

                if (ExcludedMediaProperties.Length > 0 && ExcludedMediaProperties.Contains(loweredPropertyAlias))
                {
                    continue;
                }

                writer.WritePropertyName(ToCamelCase(property.Alias));
                serializer.Serialize(writer, property.GetValue());
            }
        }

        protected virtual void WriteMediaHeirachyInformation(JsonWriter writer, IPublishedContent content)
        {
            if (content.Parent == null)
            {
                return;
            }
            writer.WritePropertyName("parentId");
            writer.WriteValue(content.Parent.Key);
        }

        protected virtual void WriteContentHeirachyInformation(JsonWriter writer, IPublishedContent content)
        {
            if (content.Children != null)
            {
                writer.WritePropertyName("children");
                writer.WriteStartArray();
                foreach (IPublishedContent child in content.Children)
                {
                    writer.WriteValue(child.Key);
                }
                writer.WriteEndArray();
            }
            if (content.Parent == null)
            {
                return;
            }
            writer.WritePropertyName("parentId");
            writer.WriteValue(content.Parent.Key);
        }

        protected virtual void WriteContentProperties(JsonWriter writer, JsonSerializer serializer, IPublishedContent content)
        {
            foreach (var property in content.Properties)
            {
                string loweredPropertyAlias = property.Alias.ToLower();
                if (ExcludedContentProperties.Length > 0 && !ExcludedContentProperties.Contains(loweredPropertyAlias))
                {
                    continue;
                }

                writer.WritePropertyName(ToCamelCase(property.Alias));
                serializer.Serialize(writer, property.GetValue());
            }
        }

        public override IPublishedContent ReadJson(JsonReader reader, Type objectType, IPublishedContent existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            throw new NotImplementedException();
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