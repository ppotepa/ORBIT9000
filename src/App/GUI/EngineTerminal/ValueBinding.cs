using Terminal.Gui;

namespace Orbit9000.EngineTerminal
{
    internal class ValueBinding
    {
        public ValueBinding (View view, object Value) 
        {
            this.View = view;
            this.Value = Value;
        }

        public View View { get; }
        public object Value { get; }
    }
}