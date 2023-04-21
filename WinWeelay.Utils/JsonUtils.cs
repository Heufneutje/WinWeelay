using System.IO;
using Newtonsoft.Json;

namespace WinWeelay.Utils
{
    /// <summary>
    /// Utility class for JSON serializing.
    /// </summary>
    public static class JsonUtils
    {
        /// <summary>
        /// Serialize and save a given object.
        /// </summary>
        /// <param name="objectToSerialize">The object to serialize.</param>
        /// <param name="path">The file path the serialized object should be saved to.</param>
        public static void SaveSerializedObject(object objectToSerialize, string path)
        {
            JsonSerializer serializer = new() { Formatting = Formatting.Indented };
            using (StreamWriter writer = File.CreateText(path))
                serializer.Serialize(writer, objectToSerialize);
        }

        /// <summary>
        /// Deserialize a given JSON string.
        /// </summary>
        /// <typeparam name="T">The type of the deserialized object.</typeparam>
        /// <param name="json">The JSON to deserialize.</param>
        /// <returns>The deserialized object.</returns>
        public static T DeserializeObject<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
