using System.IO;
using YamlDotNet.Serialization;
using Utils;

namespace Utils
{
    public static class YamlReader
    {
        private static readonly IDeserializer _deserializer =
            new DeserializerBuilder()
                .IgnoreUnmatchedProperties()
                .Build();

        public static RootCfg Load(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException("YAML config not found", path);

            var text = File.ReadAllText(path);
            var cfg = _deserializer.Deserialize<RootCfg>(text);

            if (cfg == null)
                throw new InvalidDataException("Failed to deserialize YAML config.");

            if (cfg.satellites == null || cfg.satellites.Count == 0)
                throw new InvalidDataException("No satellites found in YAML under 'satellites:'.");

            return cfg;
        }
    }

}