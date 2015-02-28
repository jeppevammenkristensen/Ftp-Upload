using System;
using System.IO;
using ConfigR;
using Newtonsoft.Json;

namespace Shared.Configuration.Json
{
    public class JsonConfigurator
    {
        public T LoadConfiguration<T>(string key, string location = "")
        {
            var path = Path.Combine(location, string.Format("{0}.json", key));

            if (File.Exists(path))
            {
                return JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
            }
            else
            {
                SaveConfiguration(key,default(T),location);
            }

            throw new InvalidOperationException(string.Format("No configuration with name {0} was found", path));
        }

        public void SaveConfiguration<T>(string key, T value,string location = "")
        {
            var path = Path.Combine(location, string.Format("{0}.json", key));

            var result = JsonConvert.SerializeObject(value);
            File.WriteAllText(path,result);
        }
    }
}