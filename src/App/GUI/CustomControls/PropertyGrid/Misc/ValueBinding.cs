namespace Terminal.Gui.CustomViews.Misc
{
    /// <summary>
    /// Provides a binding mechanism between Terminal.Gui view controls and object property values.
    /// Automatically updates the view when the bound value changes.
    /// </summary>
    public class ValueBinding
    {
        /// <summary>
        /// Gets the view that is bound to the value.
        /// </summary>
        public View View { get; }

        private object _value;

        /// <summary>
        /// Gets or sets the value bound to the view.
        /// When set, automatically updates the view content based on the value type.
        /// </summary>
        public object? Value
        {
            get => _value;
            set
            {
                _value = value!;
                switch (View)
                {
                    case Label label:
                        label.Text = value?.ToString();
                        break;
                    case TextView textView:
                        textView.Text = value?.ToString();
                        break;
                    case TextField textField:
                        textField.Text = value?.ToString();
                        break;
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the ValueBinding class.
        /// </summary>
        /// <param name="view">The view to bind to</param>
        /// <param name="value">Reference to the value being bound</param>
        public ValueBinding(View view, ref object value)
        {
            View = view;
            _value = value;
        }
    }
}
