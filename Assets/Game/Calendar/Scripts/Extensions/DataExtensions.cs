using Newtonsoft.Json;

namespace Game.Calendar.Scripts.Extensions
{
    public static class DataExtensions
    {
       public static T ToDeserialized<T>(this string json) => JsonConvert.DeserializeObject<T>(json);

        public static string ToJson(this object obj) => JsonConvert.SerializeObject(obj, Formatting.Indented);
    }
}