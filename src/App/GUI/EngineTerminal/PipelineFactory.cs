namespace EngineTerminal
{
    public class PipelineFactory
    {
        private static readonly Lazy<PipelineFactory> _instance = new(() => new PipelineFactory());
        private PipelineFactory() { }

        public static PipelineFactory Instance => _instance.Value;
        public ActionPipelineBuilder Builder => new ActionPipelineBuilder();        
    }
}
