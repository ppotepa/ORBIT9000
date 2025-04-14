namespace ORBIT9000.Core.Configuration
{
    public class OrbitAppSettings
    {
        public Orbit Orbit { get; set; }
        public Engine Engine { get; set; }
    }

    public class Orbit
    {
    }

    public class Engine
    {
        public Plugins Plugins { get; set; }
    }

    public class Plugins
    {
        public string DefaultFolder { get; set; }
        public IEnumerable<string> ActivePlugins { get; set; }
    }


}
