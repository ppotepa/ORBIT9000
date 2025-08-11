namespace EngineTerminal.Bindings
{
    using Terminal.Gui;

    public class ValueBinding
    {
        public View View { get; }
        private object _value;

        public object Value
        {
            get => _value;
            set
            {
                _value = value;
                switch (View)
                {
                    case Label label: label.Text = value?.ToString(); break;
                    case TextView textView: textView.Text = value?.ToString(); break;
                    case TextField textView: textView.Text = value?.ToString(); break;
                }
            }
        }

        public ValueBinding(View view, ref object value)
        {
            View = view;
            _value = value;
        }
    }
}
