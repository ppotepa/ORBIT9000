namespace ORBIT9000.Core.Models.Pipe
{
    namespace ORBIT9000.Core.Models.Pipe
    {
        public class ExampleData : IPipeData
        {
            [MessagePack.Key(0)]
            public SettingsData Frame1 { get; set; }

            [MessagePack.Key(1)]
            public EngineData Frame2 { get; set; }
        }

        public interface IPipeData
        {
            SettingsData Frame1 { get; set; }
            EngineData Frame2 { get; set; }
        }
    }
}