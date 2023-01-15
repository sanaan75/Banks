using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Entities
{
    public static class JsonExt
    {
        public static string ToJson(this object o)
        {
            return o == null ? null : JsonConvert.SerializeObject(o);
        }

        public static T FromJson<T>(this string json)
        {
            return string.IsNullOrWhiteSpace(json) ? default : JsonConvert.DeserializeObject<T>(json);
        }

        public static object FromJson(this string json, Type type)
        {
            return JsonConvert.DeserializeObject(json, type);
        }

        public static T GetValue<T>(string json, string path)
        {
            var obj = JObject.Parse(json);
            return obj.SelectToken(path).Value<T>();
        }
    }
}