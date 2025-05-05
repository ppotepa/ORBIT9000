using System;

namespace Terminal.Gui.CustomViews.Misc
{
    public class PipelineFactory
    {
        private static readonly Lazy<PipelineFactory> _instance = new Lazy<PipelineFactory>(() => new PipelineFactory());
        public static PipelineFactory Instance => _instance.Value;
        public ActionPipelineBuilder Builder => new ActionPipelineBuilder();
    }
}
