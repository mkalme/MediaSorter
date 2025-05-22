using System;
using System.IO;
using Newtonsoft.Json;

namespace MediaSorter {
    public static class JsonUtilities {
        public static void SaveAsset(object asset, string path) {
            using (FileStream file = File.Create(path))
            using (StreamWriter writer = new StreamWriter(file)) {
                writer.Write(JsonConvert.SerializeObject(asset, Formatting.Indented));
            }
        }
        public static TAsset ReadAsset<TAsset>(string path) {
            using (FileStream file = File.OpenRead(path))
            using (StreamReader reader = new StreamReader(file)) {
                return JsonConvert.DeserializeObject<TAsset>(reader.ReadToEnd());
            }
        }
    }
}
