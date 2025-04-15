using ORBIT9000.Engine.Configuration.Raw;
using System.Reflection;

namespace ORBIT9000.Engine.Configuration
{
    public class OrbitEngineConfig
    {
        public required Assembly[] Plugins { get; set; }
        public required DirectoryInfo DefaultFolder { get; set; }

        public bool UseDefaultFolder => this.Plugins.Length == 0;

        internal static OrbitEngineConfig? FromRaw(RawOrbitEngineConfig rawConfig)
        {
            try
            {
                if (rawConfig.OrbitEngine.Plugins.SkipMissing)
                {

                }

                return new OrbitEngineConfig
                {
                    DefaultFolder = new DirectoryInfo(rawConfig.OrbitEngine.Plugins.DefaultFolder),
                    Plugins = rawConfig.OrbitEngine.Plugins
                        .ActivePlugins
                        .Select(Assembly.LoadFile)
                        .ToArray()
                };
        }
    }
}