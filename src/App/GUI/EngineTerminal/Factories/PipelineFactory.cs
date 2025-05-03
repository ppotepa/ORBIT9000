using EngineTerminal.Builders.Pipeline;

namespace EngineTerminal.Builders
{
    public class PipelineFactory
    {
        private static readonly Lazy<PipelineFactory> _instance = new(() => new PipelineFactory());
        public static PipelineFactory Instance => _instance.Value;
        public ActionPipelineBuilder Builder => new();
    }
}