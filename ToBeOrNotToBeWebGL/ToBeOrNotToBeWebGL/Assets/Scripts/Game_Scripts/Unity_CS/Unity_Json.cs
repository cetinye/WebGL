using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Unity_CS
{
    public static class Unity_Json
    {
        public static List<T> _DeserializeList<T>(this string obj)
        {
            if (!string.IsNullOrEmpty(obj))
            {
                var convertedDeserializeObject = JsonConvert.DeserializeObject<List<T>>(obj);
                return convertedDeserializeObject;
            }
            Debug.LogWarning("There is nothing to return");
            return new List<T>();
        }

        public static T _DeserializeObject<T>(this string obj) {
            if (!string.IsNullOrEmpty(obj))
            {
                var convertedDeserializeObject = JsonConvert.DeserializeObject<T>(obj);
                return convertedDeserializeObject;
            }
            Debug.LogWarning("Your string is empty for deserialization.");
            return default;
        }
        
        public static string _SerializeList<T>(this IList<T> obj) {
            return JsonConvert.SerializeObject(obj, Formatting.Indented,
                new JsonSerializerSettings{
                    NullValueHandling = NullValueHandling.Ignore
                });
        }

        public static string _SerializeObject<T>(this T obj) {
            return JsonConvert.SerializeObject(obj, Formatting.Indented,
                new JsonSerializerSettings{
                    NullValueHandling = NullValueHandling.Ignore
                });
        }

    }
}