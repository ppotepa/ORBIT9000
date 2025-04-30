namespace EngineTerminal
{
    public class ActionFactory
    {
        private static readonly Lazy<ActionFactory> _instance = new(() => new ActionFactory());
        public static ActionFactory Instance => _instance.Value;
        private ActionFactory() { }

        public ActionBuilder Builder => new ActionBuilder();
        
    }
}
