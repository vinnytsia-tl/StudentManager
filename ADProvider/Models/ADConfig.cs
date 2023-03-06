using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;
using System.IO;

namespace ADProvider.Models
{
    public class ADConfig
    {
        public static ADConfig LoadConfig(string configFile = "config.yml")
        {
            var deserializer = new DeserializerBuilder().WithNamingConvention(UnderscoredNamingConvention.Instance).Build();
            using (var file = File.OpenText(configFile))
            {
                return deserializer.Deserialize<ADConfig>(file);
            }
        }
        public string Domain { get; set; }
        public DomainContainers Containers { get; set; }
        public DomainGroupObjects Groups { get; set; }
        public UserInfo Admin { get; set; }
    }
}
