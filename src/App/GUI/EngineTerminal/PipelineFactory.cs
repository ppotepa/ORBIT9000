namespace EngineTerminal
{
    public class PipelineFactory
    {
        private static readonly Lazy<PipelineFactory> _instance = new(() => new PipelineFactory());
        public static PipelineFactory Instance => _instance.Value;
        private PipelineFactory() { }
        public ActionPipelineBuilder Builder => new ActionPipelineBuilder();
        
    }
}
