using NStack;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Terminal.Gui.CustomViews.Misc
{
<<<<<<< HEAD
<<<<<<< HEAD
    /// <summary>
    /// Builds a pipeline of actions to process input values and apply them to a target binding.
    /// </summary>
    public class ActionPipelineBuilder
    {
        /// <summary>
        /// Stores the sequence of actions to be executed in the pipeline.
        /// </summary>
        private readonly List<Action<ustring>> _pipeline = [];

        /// <summary>
        /// The input field from which the value is retrieved.
        /// </summary>
        private TextField? _valueField;

        /// <summary>
        /// The binding that links the input value to the target property.
        /// </summary>
        private ValueBinding? _targetBinding;

        /// <summary>
        /// The parent object containing the property to be updated.
        /// </summary>
        private object? _parent;

        /// <summary>
        /// The property information of the target property to be updated.
        /// </summary>
        private PropertyInfo? _propertyInfo;

        /// <summary>
        /// Initializes the pipeline builder with the required components.
        /// </summary>
        /// <param name="valueField">The input field for the value.</param>
        /// <param name="targetBinding">The binding to the target property.</param>
        /// <param name="parent">The parent object containing the property.</param>
        /// <param name="propertyInfo">The property information of the target property.</param>
        /// <returns>The current instance of <see cref="ActionPipelineBuilder"/>.</returns>
=======
=======
    /// <summary>
    /// Builds a pipeline of actions to process input values and apply them to a target binding.
    /// </summary>
>>>>>>> 5710a06 (Add Readme)
    public class ActionPipelineBuilder
    {
        /// <summary>
        /// Stores the sequence of actions to be executed in the pipeline.
        /// </summary>
        private readonly List<Action<ustring>> _pipeline = new();

        /// <summary>
        /// The input field from which the value is retrieved.
        /// </summary>
        private TextField? _valueField;

        /// <summary>
        /// The binding that links the input value to the target property.
        /// </summary>
        private ValueBinding? _targetBinding;

        /// <summary>
        /// The parent object containing the property to be updated.
        /// </summary>
        private object? _parent;

        /// <summary>
        /// The property information of the target property to be updated.
        /// </summary>
        private PropertyInfo? _propertyInfo;

<<<<<<< HEAD
>>>>>>> e5a837c (Move Property Grid  Viewto Separate Project)
=======
        /// <summary>
        /// Initializes the pipeline builder with the required components.
        /// </summary>
        /// <param name="valueField">The input field for the value.</param>
        /// <param name="targetBinding">The binding to the target property.</param>
        /// <param name="parent">The parent object containing the property.</param>
        /// <param name="propertyInfo">The property information of the target property.</param>
        /// <returns>The current instance of <see cref="ActionPipelineBuilder"/>.</returns>
>>>>>>> 5710a06 (Add Readme)
        public ActionPipelineBuilder Create(TextField valueField, ValueBinding targetBinding, object parent, PropertyInfo propertyInfo)
        {
            _valueField = valueField;
            _targetBinding = targetBinding;
            _parent = parent;
            _propertyInfo = propertyInfo;
            _pipeline.Clear();
            _pipeline.Add(_ => ProcessInputValue());
            return this;
        }

<<<<<<< HEAD
<<<<<<< HEAD
=======
>>>>>>> 5710a06 (Add Readme)
        /// <summary>
        /// Adds a conditional action to the pipeline.
        /// </summary>
        /// <param name="condition">The condition to evaluate.</param>
        /// <param name="action">The action to execute if the condition is true.</param>
        /// <returns>The current instance of <see cref="ActionPipelineBuilder"/>.</returns>
<<<<<<< HEAD
=======
>>>>>>> e5a837c (Move Property Grid  Viewto Separate Project)
=======
>>>>>>> 5710a06 (Add Readme)
        public ActionPipelineBuilder AddIf(Func<bool> condition, Func<ValueBinding, int> action)
        {
            _pipeline.Add(_ =>
            {
                if (condition())
                    action(_targetBinding!);
            });
            return this;
        }

<<<<<<< HEAD
<<<<<<< HEAD
=======
>>>>>>> 5710a06 (Add Readme)
        /// <summary>
        /// Adds an action to the beginning of the pipeline.
        /// </summary>
        /// <param name="action">The action to add.</param>
        /// <returns>The current instance of <see cref="ActionPipelineBuilder"/>.</returns>
<<<<<<< HEAD
=======
>>>>>>> e5a837c (Move Property Grid  Viewto Separate Project)
=======
>>>>>>> 5710a06 (Add Readme)
        public ActionPipelineBuilder AddPre(Action<ustring> action)
        {
            _pipeline.Insert(0, action);
            return this;
        }

<<<<<<< HEAD
<<<<<<< HEAD
=======
>>>>>>> 5710a06 (Add Readme)
        /// <summary>
        /// Adds an action to the end of the pipeline.
        /// </summary>
        /// <param name="action">The action to add.</param>
        /// <returns>The current instance of <see cref="ActionPipelineBuilder"/>.</returns>
<<<<<<< HEAD
=======
>>>>>>> e5a837c (Move Property Grid  Viewto Separate Project)
=======
>>>>>>> 5710a06 (Add Readme)
        public ActionPipelineBuilder AddPost(Action<ustring> action)
        {
            _pipeline.Add(action);
            return this;
        }

<<<<<<< HEAD
<<<<<<< HEAD
=======
>>>>>>> 5710a06 (Add Readme)
        /// <summary>
        /// Builds the pipeline into a single executable action.
        /// </summary>
        /// <returns>An action that executes all steps in the pipeline.</returns>
<<<<<<< HEAD
        public Action<ustring> Build() => input =>
        {
            foreach (Action<ustring> step in _pipeline)
                step(input);
        };

        /// <summary>
        /// Processes the input value and applies it to the target property.
        /// </summary>
=======
=======
>>>>>>> 5710a06 (Add Readme)
        public Action<ustring> Build() => input =>
        {
            foreach (var step in _pipeline)
                step(input);
        };

<<<<<<< HEAD
>>>>>>> e5a837c (Move Property Grid  Viewto Separate Project)
=======
        /// <summary>
        /// Processes the input value and applies it to the target property.
        /// </summary>
>>>>>>> 5710a06 (Add Readme)
        private void ProcessInputValue()
        {
            if (_valueField == null || _targetBinding == null || _parent == null || _propertyInfo == null)
                return;

<<<<<<< HEAD
            string? text = _valueField.Text.ToString();
            switch (Type.GetTypeCode(_propertyInfo.PropertyType))
            {
                case TypeCode.String:
                    ApplyValue(text!); break;
                case TypeCode.Int32:
                    if (int.TryParse(text, out int i)) ApplyValue(i);
                    break;

                case TypeCode.Boolean:
                    if (bool.TryParse(text, out bool b)) ApplyValue(b);
=======
            var text = _valueField.Text.ToString();
            switch (Type.GetTypeCode(_propertyInfo.PropertyType))
            {
                case TypeCode.String:
                    ApplyValue(text, text); break;
                case TypeCode.Int32:
                    if (int.TryParse(text, out var i)) ApplyValue(i, text);
                    break;

                case TypeCode.Boolean:
                    if (bool.TryParse(text, out var b)) ApplyValue(b, text);
>>>>>>> e5a837c (Move Property Grid  Viewto Separate Project)
                    break;
            }
        }

<<<<<<< HEAD
<<<<<<< HEAD
        /// <summary>
        /// TODO: THERE IS SOMETHING WRONG WITH THIS METHOD
        /// Applies the validated value to the target property and binding.
        /// </summary>
        /// <param name="value">The value to apply.</param>
        private void ApplyValue(object value)
        {
            if (_propertyInfo == null || _parent == null || _targetBinding == null || value == null) return;
=======
=======
        /// <summary>
        /// Applies the validated value to the target property and binding.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="value">The value to apply.</param>
        /// <param name="original">The original string representation of the value.</param>
>>>>>>> 5710a06 (Add Readme)
        private void ApplyValue<T>(T value, string original)
        {
            if (_propertyInfo == null || _parent == null || _targetBinding == null) return;
>>>>>>> e5a837c (Move Property Grid  Viewto Separate Project)
            if (Validate(_propertyInfo, _parent, value))
            {
                _targetBinding.Value = value!;
                _propertyInfo.SetValue(_parent, value);
            }
        }

<<<<<<< HEAD
<<<<<<< HEAD
=======
>>>>>>> 5710a06 (Add Readme)
        /// <summary>
        /// Validates the value against the property's validation attributes.
        /// </summary>
        /// <param name="prop">The property to validate.</param>
        /// <param name="parent">The parent object containing the property.</param>
        /// <param name="value">The value to validate.</param>
        /// <returns>True if the value is valid; otherwise, false.</returns>
<<<<<<< HEAD
        private static bool Validate(PropertyInfo prop, object parent, object value)
        {
            IEnumerable<ValidationAttribute> attributes = prop.GetCustomAttributes<ValidationAttribute>();
=======
=======
>>>>>>> 5710a06 (Add Readme)
        private bool Validate(PropertyInfo prop, object parent, object value)
        {
            var attributes = prop.GetCustomAttributes<ValidationAttribute>();
>>>>>>> e5a837c (Move Property Grid  Viewto Separate Project)

            if (!attributes.Any())
            {
                return true;
            }

<<<<<<< HEAD
            ValidationContext context = new(parent) { MemberName = prop.Name };

            foreach (ValidationAttribute attr in attributes)
=======
            var context = new ValidationContext(parent) { MemberName = prop.Name };

            foreach (var attr in attributes)
>>>>>>> e5a837c (Move Property Grid  Viewto Separate Project)
            {
                ValidationResult? res = attr.GetValidationResult(value, context);
                if (res != ValidationResult.Success)
                {
                    //HandleFailure(attr, prop, parent, ref value);
                    return false;
                }
            }

            return true;
        }

        //private void HandleFailure(ValidationAttribute attr, PropertyInfo prop, object parent, ref object value)
        //{
        //    if (attr is MaxValueAttribute max)
        //    {
        //        value = max.MaxValue;
        //        prop.SetValue(parent, value);
        //        MessageBox.ErrorQuery("Validation Error", $"Reset to max: {max.MaxValue}", "OK");
        //        _valueField!.Text = value.ToString();
        //    }
        //    else
        //    {
        //        MessageBox.ErrorQuery("Validation Error", attr.ErrorMessage ?? "Invalid", "OK");
        //    }
        //}
    }
}