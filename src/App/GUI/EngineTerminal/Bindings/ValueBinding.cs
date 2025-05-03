using Terminal.Gui;

namespace EngineTerminal.Bindings
{
    public class ValueBinding
    {
        public ValueBinding(View view, ref object value)
        {
            this.View = view;
            this._value = value;
        }

        public View View { get; }

        private object _value;
        public object Value
        {
            get => _value;
            set
            {
                _value = value;

                if (View is Label label)
                {
                    label.Text = value?.ToString();
                }

                if (View is TextView textView)
                {
                    textView.Text = value?.ToString();
                }

                if (View is TextField textField)
                {
                    textField.Text = value?.ToString();
                }
            }
        }
    }
}