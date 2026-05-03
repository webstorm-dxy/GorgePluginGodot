using System;
using Gorge.Native.GorgeFramework;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Gorge.GorgeFramework.Utilities.Json
{
    public class GorgeVector2Converter : JsonConverter<Vector2>
    {
        public override Vector2 ReadJson(JsonReader reader, Type objectType,
            Vector2 existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            var jObject = JObject.Load(reader);
            var x = jObject["x"]?.ToObject<float>() ?? 0;
            var y = jObject["y"]?.ToObject<float>() ?? 0;
            return new Vector2(x, y);
        }

        public override void WriteJson(JsonWriter writer, Vector2 value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
            }
            else
            {
                writer.WriteStartObject();
                writer.WritePropertyName("x");
                writer.WriteValue(value.x);
                writer.WritePropertyName("y");
                writer.WriteValue(value.y);
                writer.WriteEndObject();
            }
        }
    }
}