/*
 * Developer: Ramtin Jokar [ Ramtinak@live.com ] [ My Telegram Account: https://t.me/ramtinak ]
 * 
 * Github source: https://github.com/ramtinak/InstagramApiSharp
 * Nuget package: https://www.nuget.org/packages/InstagramApiSharp
 * 
 * IRANIAN DEVELOPERS
 */

using System;
using System.Linq;
using InstaSharper.Classes.ResponseWrappers.Hashtags;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace InstaSharper.Converters.Json
{
    internal class InstaHashtagSuggestedDataConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(InstaHashtagSearchResponse);
        }

        public override object ReadJson(JsonReader reader,
            Type objectType,
            object existingValue,
            JsonSerializer serializer)
        {
            var token = JToken.Load(reader);
            var container = token["tags"];
            var tags = token.ToObject<InstaHashtagSearchResponse>();
            if (container != null && container.Any())
            {
                foreach (var item in container)
                {
                    try
                    {
                        tags.Tags.Add(item.ToObject<InstaHashtagResponse>());
                    }
                    catch { }
                }
            }
            return tags;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }
}
