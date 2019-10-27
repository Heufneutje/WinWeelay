using Newtonsoft.Json;
using System.IO;

namespace WinWeelay.Utils
{
    public static class JsonUtils
    {
        public static void SaveSerializedObject(object objectToSerialize, string path)
        {
            JsonSerializer serializer = new JsonSerializer { Formatting = Formatting.Indented };
            using (StreamWriter writer = File.CreateText(path))
                serializer.Serialize(writer, objectToSerialize);
        }

        public static T DeserializeObject<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static object DeserializeObject(string json)
        {
            return JsonConvert.DeserializeObject(json);
        }
    }
}
