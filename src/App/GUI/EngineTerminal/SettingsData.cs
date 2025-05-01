namespace Orbit9000.EngineTerminal
{
    public class EngineData
    {
        public bool IsValid { get; set; }

        [MaxValue(100)] 
        public int Setting1 { get; set; }

        [MaxValue(15)]

        public int Setting2 { get; set; }
    }

    public class SettingsData
    {
        private string setting1;

        public string Setting1
        {
            get { return setting1; }
            set { this.setting1 = value; }
        }

        public int Setting10 { get; set; }
        public int Setting11 { get; set; }
        public int Setting12 { get; set; }
        public string Setting2 { get; set; }
        public string Setting3 { get; set; }
        public string Setting4 { get; set; }
        public string Setting5 { get; set; }
        public string Setting6 { get; set; }
        public int Setting7 { get; set; }
        public int Setting8 { get; set; }
        public int Setting9 { get; set; }
    }
}