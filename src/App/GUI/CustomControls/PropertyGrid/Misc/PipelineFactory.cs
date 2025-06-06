namespace Terminal.Gui.CustomViews.Misc
{
    /// <summary>
    /// Factory for creating action pipelines that process UI input events.
    /// Implements the singleton pattern for global access.
    /// </summary>
    public sealed class PipelineFactory
    {
        /// <summary>
        /// Gets a new ActionPipelineBuilder for creating action pipelines.
        /// </summary>
        public static ActionPipelineBuilder Builder => new();

        // Private constructor to prevent instantiation from outside
        private PipelineFactory() { }
    }
}
