
namespace ORBIT9000.Core.Models.Pipe
{
    namespace ORBIT9000.Core.Models.Pipe
    {
        public class ExampleData : IPipeData
        {
            public ExampleData()
            {

            }

            public ExampleData(SettingsData frame1, EngineData frame2)
            {
                this.Frame1 = frame1 ?? throw new ArgumentNullException(nameof(frame1));
                this.Frame2 = frame2 ?? throw new ArgumentNullException(nameof(frame2));
            }

            [MessagePack.Key(0)]
            public SettingsData? Frame1 { get; set; }

            [MessagePack.Key(1)]
            public EngineData? Frame2 { get; set; }
        }

        public interface IPipeData
        {
            SettingsData? Frame1 { get; set; }
            EngineData? Frame2 { get; set; }
        }
    }
}