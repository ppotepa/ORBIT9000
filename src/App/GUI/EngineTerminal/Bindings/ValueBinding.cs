using Terminal.Gui;

namespace EngineTerminal.Bindings
{
    public class ValueBinding
    {
        public ValueBinding (View view, ref object Value) 
        {
            this.View = view;
            this.Value = Value;
        }

        public View View { get; }
        public object Value { get; set; }
    }
}