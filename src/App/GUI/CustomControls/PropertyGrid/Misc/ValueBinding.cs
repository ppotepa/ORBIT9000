namespace Terminal.Gui.CustomViews.Misc
{
<<<<<<< HEAD
<<<<<<< HEAD
=======
>>>>>>> 5710a06 (Add Readme)
    /// <summary>
    /// Provides a binding mechanism between Terminal.Gui view controls and object property values.
    /// Automatically updates the view when the bound value changes.
    /// </summary>
<<<<<<< HEAD
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
=======
=======
>>>>>>> 5710a06 (Add Readme)
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
<<<<<<< HEAD
        public object Value
>>>>>>> e5a837c (Move Property Grid  Viewto Separate Project)
=======
        public object? Value
>>>>>>> bfa6c2d (Try fix pipeline)
        {
            get => this._value;
            set
            {
<<<<<<< HEAD
<<<<<<< HEAD
                _value = value!;
=======
                _value = value;
>>>>>>> e5a837c (Move Property Grid  Viewto Separate Project)
                switch (View)
=======
                this._value = value!;
                switch (this.View)
>>>>>>> bfa6c2d (Try fix pipeline)
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

<<<<<<< HEAD
<<<<<<< HEAD
=======
>>>>>>> 5710a06 (Add Readme)
        /// <summary>
        /// Initializes a new instance of the ValueBinding class.
        /// </summary>
        /// <param name="view">The view to bind to</param>
        /// <param name="value">Reference to the value being bound</param>
<<<<<<< HEAD
=======
>>>>>>> e5a837c (Move Property Grid  Viewto Separate Project)
=======
>>>>>>> 5710a06 (Add Readme)
        public ValueBinding(View view, ref object value)
        {
            this.View = view;
            this._value = value;
        }
    }
}
